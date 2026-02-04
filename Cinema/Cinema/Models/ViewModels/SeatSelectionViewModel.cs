using Cinema.Models;

namespace Cinema.Models.ViewModels
{
    public class SeatSelectionViewModel
    {
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        
        // Formato: "A-1", "B-5" - assentos ocupados
        public List<string> OccupiedSeats { get; set; } = new List<string>();

        // Dados para a compra
        public string SelectedSeatRow { get; set; } = null!;
        public int SelectedSeatNumber { get; set; }
        
        // Se for vendedor/admin comprando para outro pessoa
        public int? ClientId { get; set; } 
    }
}
