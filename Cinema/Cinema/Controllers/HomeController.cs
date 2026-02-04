using System.Diagnostics;
using Cinema.Data;
using Cinema.Repository;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly ISessionRepository _sessionRepository;

        private readonly ITicketRepository _ticketRepository;

        public HomeController(
            ILogger<HomeController> logger, IMovieRepository movieRepository, ISessionRepository sessionRepository, ITicketRepository ticketRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _sessionRepository = sessionRepository;
            _ticketRepository = ticketRepository;
        }
        
        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAll();
            var sessions = await _sessionRepository.GetAll();

            // Filtrar sessões futuras
            var futureSessions = sessions.Where(s => s.StartTime > DateTime.Now).ToList();

            // Construir ViewModel
            var viewModel = new Cinema.Models.ViewModels.HomeViewModel();

            foreach (var movie in movies)
            {
                var movieSessions = futureSessions
                    .Where(s => s.MovieID == movie.ID)
                    .OrderBy(s => s.StartTime)
                    .ToList();

                if (movieSessions.Any())
                {
                    // Filme em Cartaz
                    viewModel.NowShowing.Add(new Cinema.Models.ViewModels.MovieScheduleViewModel
                    {
                        Movie = movie,
                        Sessions = movieSessions
                    });
                }
                else
                {
                    // Em Breve (Filme existe mas sem sessões futuras)
                    viewModel.ComingSoon.Add(movie);
                }
            }

            // Featured: Filmes em cartaz com imagem (Top 5)
            viewModel.FeaturedMovies = viewModel.NowShowing
                .Where(m => !string.IsNullOrEmpty(m.Movie.ImageURL))
                .Select(m => m.Movie)
                .Take(5)
                .ToList();

            // Fallback: Se não tiver em cartaz, pega do Em Breve para não ficar vazio
            if (!viewModel.FeaturedMovies.Any())
            {
                viewModel.FeaturedMovies = viewModel.ComingSoon
                    .Where(m => !string.IsNullOrEmpty(m.ImageURL))
                    .Take(5)
                    .ToList();
            }

            return View(viewModel);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var movies = await _movieRepository.GetAll();
            var sessions = await _sessionRepository.GetAll();
            var tickets = await _ticketRepository.GetAll(); // Ideal seria ter queries especificas no repo, mas para MVP ok

            // Métricas KPI
            ViewBag.TotalMovies = movies.Count;
            ViewBag.TotalSessions = sessions.Count;
            ViewBag.TicketsSold = tickets.Count;
            ViewBag.Revenue = tickets.Sum(t => t.Price ?? 0); // Assumindo que Ticket tem Price gravado

            // Dados para Gráfico: Ingressos por Filme
            // Agrupar ingressos por Session.MovieID
            var ticketsByMovie = tickets
                .GroupBy(t => t.Session?.Movie?.Title ?? "Desconhecido")
                .Select(g => new { Movie = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            ViewBag.ChartLabels = ticketsByMovie.Select(x => x.Movie).ToArray();
            ViewBag.ChartData = ticketsByMovie.Select(x => x.Count).ToArray();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
