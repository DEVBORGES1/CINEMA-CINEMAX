using Cinema.Data;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Cinema.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CinemaContext _context;

        public CheckoutController(CinemaContext context)
        {
            _context = context;
        }

        // ENTRY POINT: Initialize checkout for a session
        public async Task<IActionResult> Index(int sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.ID == sessionId);

            if (session == null)
            {
                return NotFound();
            }

            // Initialize minimal state
            var checkoutState = new CheckoutViewModel
            {
                SessionID = session.ID,
                Session = session,
                QtyNormal = 1 // Default
            };

            // Save to Session
            HttpContext.Session.Set("CheckoutState", checkoutState);

            return RedirectToAction("Step1");
        }

        // STEP 1: Type & Quantity
        [HttpGet]
        public IActionResult Step1()
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // Reload Session from DB to ensure details are present
            state.Session = _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefault(s => s.ID == state.SessionID);

            return View(state);
        }

        [HttpPost]
        public IActionResult Step1(int qtyNormal, int qtyStudent)
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            if (qtyNormal + qtyStudent <= 0)
            {
                ModelState.AddModelError("", "Selecione pelo menos um ingresso.");
                // Reload session for View
                 state.Session = _context.Sessions.Include(s => s.Movie).Include(s => s.Room).FirstOrDefault(s => s.ID == state.SessionID);
                return View(state);
            }

            // Update State
            state.QtyNormal = qtyNormal;
            state.QtyStudent = qtyStudent;
            
            // Calc Total Price (Pre-Seat)
            // Note: In a real app we'd fetch price from DB. Assuming Session.Price is base.
            // We need to re-fetch session price since it's not serialized fully if circular.
            var sessionPrice = _context.Sessions.Where(s => s.ID == state.SessionID).Select(s => s.Price).FirstOrDefault();
            
            // Logic: Normal = Price, Student = Half
            state.TotalPrice = (qtyNormal * sessionPrice) + (qtyStudent * (sessionPrice * 0.5m));

            // Save & Next
            HttpContext.Session.Set("CheckoutState", state);
            return RedirectToAction("Step2");
        }
        
        // STEP 2: Seat Selection
        [HttpGet]
        public async Task<IActionResult> Step2()
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // Load Session with Room info to know dimensions
            state.Session = await _context.Sessions
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.ID == state.SessionID);

            // Get taken seats
            var takenSeats = await _context.Tickets
                .Where(t => t.SessionID == state.SessionID)
                .Select(t => t.SeatRow + t.SeatNumber) // e.g. "A1"
                .ToListAsync();
            
            // Pass taken seats to View using ViewBag (or ViewModel if we added a property)
            // Ideally we should add to ViewModel, but for now ViewBag is easier to not break contract
            ViewBag.TakenSeats = takenSeats;

            return View(state);
        }

        [HttpPost]
        public IActionResult Step2(string selectedSeatsJson)
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // Parse selected seats (comma separated or JSON)
            // Simplest: Hidden input with comma separated values "A1,B2"
            var seats = selectedSeatsJson?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            if (seats.Count != state.TotalTickets)
            {
                // Reload data for view
                state.Session = _context.Sessions.Include(s => s.Room).Include(s => s.Movie).FirstOrDefault(s => s.ID == state.SessionID);
                ViewBag.TakenSeats = _context.Tickets.Where(t => t.SessionID == state.SessionID).Select(t => t.SeatRow + t.SeatNumber).ToList();
                ModelState.AddModelError("", $"Selecione exatamente {state.TotalTickets} assento(s).");
                return View(state);
            }

            state.SelectedSeats = seats;
            HttpContext.Session.Set("CheckoutState", state);

            return RedirectToAction("Step3");
        }
        
        // STEP 3: Snacks
        [HttpGet]
        public async Task<IActionResult> Step3()
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // Load Snacks from DB
            var allSnacks = await _context.Snacks.ToListAsync();
            
            // Pass to View using ViewBag (simplest for now, or map to ViewModel)
            ViewBag.AllSnacks = allSnacks;

            return View(state);
        }

        [HttpPost]
        public IActionResult Step3(Dictionary<int, int> snacks)
        {
            // snacks: ID -> Quantity
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // Process selected snacks
            state.SelectedSnacks = new List<SnackSelectionViewModel>();
            
            // We need prices to calculate total
            var dbSnacks = _context.Snacks.ToList(); // Determine prices

            foreach (var item in snacks)
            {
                if (item.Value > 0)
                {
                    var snack = dbSnacks.FirstOrDefault(s => s.ID == item.Key);
                    if (snack != null)
                    {
                        state.SelectedSnacks.Add(new SnackSelectionViewModel
                        {
                            SnackID = snack.ID,
                            Name = snack.Name,
                            Price = snack.Price,
                            ImageURL = snack.ImageURL,
                            Quantity = item.Value
                        });
                    }
                }
            }
            
            // Recalculate Total Price
            // Ticket Price
            var sessionPrice = _context.Sessions.Where(s => s.ID == state.SessionID).Select(s => s.Price).FirstOrDefault();
            decimal ticketTotal = (state.QtyNormal * sessionPrice) + (state.QtyStudent * (sessionPrice * 0.5m));
            
            // Snack Price
            decimal snackTotal = state.SelectedSnacks.Sum(s => s.Price * s.Quantity);
            
            state.TotalPrice = ticketTotal + snackTotal;

            HttpContext.Session.Set("CheckoutState", state);
            return RedirectToAction("Step4");
        }
        
        // STEP 4: Payment & Summary
        [HttpGet]
        public async Task<IActionResult> Step4()
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // Ensure Session Details are loaded
            state.Session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.ID == state.SessionID);

            return View(state);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm()
        {
            var state = HttpContext.Session.Get<CheckoutViewModel>("CheckoutState");
            if (state == null) return RedirectToAction("Index", "Home");

            // 1. Create Order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = state.TotalPrice,
                // PersonID could be linked if user is logged in
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Get Order ID

            // 2. Create Tickets
            foreach (var seat in state.SelectedSeats)
            {
                // Parse "A1" -> Row "A", Number 1
                string row = seat.Substring(0, 1);
                int number = int.Parse(seat.Substring(1));

                var ticket = new Ticket
                {
                    SessionID = state.SessionID,
                    OrderID = order.ID,
                    Price = state.Session?.Price, // Simplification: Not tracking exact price per ticket type here properly in DB unless we split details
                    SeatRow = row,
                    SeatNumber = number,
                    PurchaseDate = DateTime.Now
                };
                
                // Logic to set specific price if needed, but for now using session base or average
                _context.Tickets.Add(ticket);
            }

            // 3. Create Snack Orders
            foreach (var snack in state.SelectedSnacks)
            {
                var snackOrder = new SnackOrder
                {
                    OrderID = order.ID,
                    SnackID = snack.SnackID,
                    Quantity = snack.Quantity,
                    PriceAtPurchase = snack.Price
                };
                _context.SnackOrders.Add(snackOrder);
            }

            await _context.SaveChangesAsync();
            
            // Clear Session
            HttpContext.Session.Remove("CheckoutState");

            return RedirectToAction("Confirmation", new { orderId = order.ID });
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Tickets).ThenInclude(t => t.Session).ThenInclude(s => s.Movie)
                .Include(o => o.Snacks).ThenInclude(s => s.Snack)
                .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}

