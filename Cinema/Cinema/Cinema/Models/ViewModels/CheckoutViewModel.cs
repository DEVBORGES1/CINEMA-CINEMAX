using Cinema.Models;

namespace Cinema.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public int SessionID { get; set; }
        public Session? Session { get; set; }
        
        // Step 1: Ticket Quantities
        public int QtyNormal { get; set; }
        public int QtyStudent { get; set; }
        
        public int TotalTickets => QtyNormal + QtyStudent;
        
        // Step 2: Selected Seats
        public List<string> SelectedSeats { get; set; } = new List<string>();
        
        // Step 3: Snacks
        public List<SnackSelectionViewModel> Snacks { get; set; } = new List<SnackSelectionViewModel>();
        public List<SnackSelectionViewModel> SelectedSnacks { get; set; } = new List<SnackSelectionViewModel>();
        
        public decimal TotalPrice { get; set; }
    }

    public class SnackSelectionViewModel
    {
        public int SnackID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; } = string.Empty;
    }
}
