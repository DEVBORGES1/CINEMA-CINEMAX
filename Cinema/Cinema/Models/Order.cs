using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public int? PersonID { get; set; } // Optional: Link to registered user
        [ForeignKey(nameof(PersonID))]
        public Person? Person { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<SnackOrder> Snacks { get; set; } = new List<SnackOrder>();
    }
}
