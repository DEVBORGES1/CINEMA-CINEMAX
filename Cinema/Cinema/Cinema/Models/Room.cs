using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Room
    {
        [Key]
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public int Capacity { get; set; }
        public int Rows { get; set; } = 8;
        public int Columns { get; set; } = 10;
    }
}