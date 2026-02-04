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
        
        // Placeholder for Next Steps
        public IActionResult Step2()
        {
            return Content("Step 2: Seat Selection (Coming Soon)");
        }
    }
}
