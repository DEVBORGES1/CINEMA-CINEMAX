using Cinema.Models;
using Cinema.Models.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CinemaContext context)
        {
            // NÃO chamar EnsureCreated() quando usamos migrations. O Program.cs já chama Database.Migrate().

            // Seed AgeRating se vazio
            // Usar context.Set<AgeRating>() para não depender de uma propriedade DbSet chamada AgeRating
            if (!context.Set<AgeRating>().Any())
            {
                var ages = new AgeRating[]
                {
                    new AgeRating { Rating = "Livre" },
                    new AgeRating { Rating = "10" },
                    new AgeRating { Rating = "12" },
                    new AgeRating { Rating = "14" },
                    new AgeRating { Rating = "16" },
                    new AgeRating { Rating = "18" }
                };
                context.Set<AgeRating>().AddRange(ages);
                context.SaveChanges();
            }

            // Seed Roles se vazio (Cliente, Ator, Diretor, Estudante)
            if (!context.Roles.Any())
            {
                var roles = new Role[]
                {
                    new Role { Name = "Cliente" },
                    new Role { Name = "Ator" },
                    new Role { Name = "Diretor" },
                    new Role { Name = "Estudante" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // Seed Filmes
            if (!context.Movies.Any())
            {
                var action = context.Genres.FirstOrDefault(g => g.Name == "Ação");
                var scifi = context.Genres.FirstOrDefault(g => g.Name == "Ficção Científica");
                var drama = context.Genres.FirstOrDefault(g => g.Name == "Drama");

                var rating14 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "14");
                var rating12 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "12");

                var movies = new List<Movie>();

                // Filme 1: Inception
                if (action != null && scifi != null)
                {
                    movies.Add(new Movie
                    {
                        Title = "A Origem",
                        Synopsis = "Dom Cobb é um ladrão habilidoso, o melhor absoluto na perigosa arte da extração, roubando segredos valiosos das profundezas do subconsciente durante o estado de sonho.",
                        DurationInMinutes = 148,
                        ImageURL = "https://image.tmdb.org/t/p/original/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg", // Poster URL real
                        AgeRating = rating14,
                        Genres = new List<Genre> { action, scifi }
                    });
                }

                // Filme 2: Vingadores
                if (action != null)
                {
                    movies.Add(new Movie
                    {
                        Title = "Os Vingadores",
                        Synopsis = "Os heróis mais poderosos da Terra, incluindo Homem de Ferro, Capitão América, Thor e Hulk, se unem para combater um inimigo inesperado.",
                        DurationInMinutes = 143,
                        ImageURL = "https://image.tmdb.org/t/p/original/u3bZgnGQ9TWA758r89C7Jkm065.jpg",
                        AgeRating = rating12,
                        Genres = new List<Genre> { action }
                    });
                }
                
                // Filme 3: O Poderoso Chefão
                if (drama != null)
                {
                    movies.Add(new Movie
                    {
                        Title = "O Poderoso Chefão",
                        Synopsis = "O patriarca idoso de uma dinastia do crime organizado transfere o controle de seu império clandestino para seu filho relutante.",
                        DurationInMinutes = 175,
                        ImageURL = "https://image.tmdb.org/t/p/original/oJagOzBu9Rdd9BrcimcMmFZWpZs.jpg",
                        AgeRating = rating14, // Aprox
                        Genres = new List<Genre> { drama }
                    });
                }

                context.Movies.AddRange(movies);
                context.SaveChanges();
            }

            // Seed Snacks
            if (!context.Snacks.Any())
            {
                var snacks = new Snack[]
                {
                    new Snack { Name = "Pipoca Pequena", Price = 15.00m, Description = "Salgada ou Doce", ImageURL = "https://cdn-icons-png.flaticon.com/512/579/579997.png" },
                    new Snack { Name = "Pipoca Grande", Price = 25.00m, Description = "Ideal para dividir", ImageURL = "https://cdn-icons-png.flaticon.com/512/3063/3063065.png" },
                    new Snack { Name = "Combo Duo", Price = 45.00m, Description = "2 Refs + 1 Pipoca Grande", ImageURL = "https://cdn-icons-png.flaticon.com/512/3507/3507102.png" },
                    new Snack { Name = "Refrigerante 500ml", Price = 8.00m, Description = "Cola, Guaraná ou Limão", ImageURL = "https://cdn-icons-png.flaticon.com/512/2405/2405597.png" },
                    new Snack { Name = "Água", Price = 5.00m, Description = "Sem Gás", ImageURL = "https://cdn-icons-png.flaticon.com/512/824/824239.png" },
                    new Snack { Name = "Chocolate", Price = 7.00m, Description = "Barra ao Leite", ImageURL = "https://cdn-icons-png.flaticon.com/512/2234/2234972.png" }
                };
                context.Snacks.AddRange(snacks);
                context.SaveChanges();
            }


            if (!context.Rooms.Any())
            {
                var rooms = new Room[]
                {
                    new Room { RoomNumber = 1, Capacity = 50 },
                    new Room { RoomNumber = 2, Capacity = 60 },
                    new Room { RoomNumber = 3, Capacity = 40 },
                    new Room { RoomNumber = 4, Capacity = 80 }
                };
                context.Rooms.AddRange(rooms);
                context.SaveChanges();
            }

            if (!context.Genres.Any())
            {
                var genres = new Genre[]
                {
                    new Genre { Name = "Ação" },
                    new Genre { Name = "Comédia" },
                    new Genre { Name = "Drama" },
                    new Genre { Name = "Romance" },
                    new Genre { Name = "Terror" },
                    new Genre { Name = "Ficção Científica" },
                    new Genre { Name = "Animação" },
                    new Genre { Name = "Documentário" },
                    new Genre { Name = "Suspense" },
                    new Genre { Name = "Fantasia" },
                    new Genre { Name = "Aventura" },
                    new Genre { Name = "Musical" }
                };
                context.Genres.AddRange(genres);
                context.SaveChanges();
            }

            // Seed Users if not exists
            if (!context.Persons.Any(p => p.Email == "admin@cinemax.com"))
            {
                var admin = new Person
                {
                    FirstName = "Admin",
                    LastName = "System",
                    Email = "admin@cinemax.com",
                    CPF = "00000000000",
                    Role = UserRole.Admin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    BirthDate = new DateOnly(1990, 1, 1)
                };
                context.Persons.Add(admin);
            }

            if (!context.Persons.Any(p => p.Email == "vendedor@cinemax.com"))
            {
                var seller = new Person
                {
                    FirstName = "Vendedor",
                    LastName = "Um",
                    Email = "vendedor@cinemax.com",
                    CPF = "11111111111",
                    Role = UserRole.Seller,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    BirthDate = new DateOnly(1995, 1, 1)
                };
                context.Persons.Add(seller);
            }

            if (!context.Persons.Any(p => p.Email == "cliente@cinemax.com"))
            {
                var client = new Person
                {
                    FirstName = "Cliente",
                    LastName = "Teste",
                    Email = "cliente@cinemax.com",
                    CPF = "22222222222",
                    Role = UserRole.Client,
                    IsClient = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    BirthDate = new DateOnly(2000, 1, 1)
                };
                context.Persons.Add(client);
            }

            // Seed Sessões (Para hoje e amanhã)
            if (!context.Sessions.Any())
            {
                var movie1 = context.Movies.FirstOrDefault(m => m.Title == "A Origem");
                var movie2 = context.Movies.FirstOrDefault(m => m.Title == "Os Vingadores");
                var movie3 = context.Movies.FirstOrDefault(m => m.Title == "O Poderoso Chefão");
                
                var room1 = context.Rooms.FirstOrDefault(r => r.RoomNumber == 1);
                var room2 = context.Rooms.FirstOrDefault(r => r.RoomNumber == 2);

                if (movie1 != null && room1 != null)
                {
                    // Hoje
                    context.Sessions.Add(new Session { Movie = movie1, Room = room1, StartTime = DateTime.Now.Date.AddHours(14), Price = 25.00m });
                    context.Sessions.Add(new Session { Movie = movie1, Room = room1, StartTime = DateTime.Now.Date.AddHours(18), Price = 28.00m });
                    // Amanhã
                    context.Sessions.Add(new Session { Movie = movie1, Room = room1, StartTime = DateTime.Now.Date.AddDays(1).AddHours(20), Price = 30.00m });
                }

                if (movie2 != null && room2 != null)
                {
                    // Hoje
                    context.Sessions.Add(new Session { Movie = movie2, Room = room2, StartTime = DateTime.Now.Date.AddHours(15), Price = 25.00m });
                    // Amanhã
                    context.Sessions.Add(new Session { Movie = movie2, Room = room2, StartTime = DateTime.Now.Date.AddDays(1).AddHours(15), Price = 25.00m });
                }

                 if (movie3 != null && room1 != null)
                {
                    // Hoje
                    context.Sessions.Add(new Session { Movie = movie3, Room = room1, StartTime = DateTime.Now.Date.AddHours(21), Price = 35.00m });
                }

                context.SaveChanges();
            }

            // Seed Tickets (Alguns vendidos para popular dashboard)
            if (!context.Tickets.Any())
            {
                var session = context.Sessions.Include(s => s.Movie).FirstOrDefault();
                var client = context.Persons.FirstOrDefault(p => p.Email == "cliente@cinemax.com");

                if (session != null && client != null)
                {
                    context.Tickets.Add(new Ticket { Session = session, Person = client, SeatRow = "A", SeatNumber = 1, PurchaseDate = DateTime.Now.AddHours(-1), Price = session.Price });
                    context.Tickets.Add(new Ticket { Session = session, Person = client, SeatRow = "A", SeatNumber = 2, PurchaseDate = DateTime.Now.AddHours(-1), Price = session.Price });
                    context.SaveChanges();
                }
            }

            context.SaveChanges();
        }
    }
}