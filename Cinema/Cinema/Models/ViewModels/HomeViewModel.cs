using Cinema.Models;

namespace Cinema.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<MovieScheduleViewModel> NowShowing { get; set; } = new List<MovieScheduleViewModel>();
        public List<Movie> ComingSoon { get; set; } = new List<Movie>();
        public List<Movie> FeaturedMovies { get; set; } = new List<Movie>();
    }
}
