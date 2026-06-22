using Cinema.Models;

namespace Cinema.Models.ViewModels
{
    public class MovieScheduleViewModel
    {
        public Movie Movie { get; set; } = null!;
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
