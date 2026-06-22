using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class SnackOrder
    {
        public int ID { get; set; }
        
        public int OrderID { get; set; }
        [ForeignKey(nameof(OrderID))]
        public Order? Order { get; set; }

        public int SnackID { get; set; }
        [ForeignKey(nameof(SnackID))]
        public Snack? Snack { get; set; }

        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; } // Snapshot of price
    }
}
