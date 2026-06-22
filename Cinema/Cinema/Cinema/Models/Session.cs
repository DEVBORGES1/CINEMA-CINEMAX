using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Session
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MovieID { get; set; }
        
        [ForeignKey(nameof(MovieID))]
        public Movie? Movie { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 30.0m;

        public int RoomID { get; set; }
        
        [ForeignKey(nameof(RoomID))]
        public Room? Room { get; set; }
        public List<Ticket>? Tickets { get; set; }
    }
}
