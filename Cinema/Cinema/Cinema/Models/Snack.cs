using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Snack
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public string ImageURL { get; set; } = string.Empty;
    }
}
