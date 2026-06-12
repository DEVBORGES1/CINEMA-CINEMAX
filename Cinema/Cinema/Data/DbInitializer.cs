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

            // Seed Genres se vazio (precisa ser feito ANTES de Seed Filmes!)
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

            // Seed Filmes (Verificar individualmente para poder semear novos filmes)
            var action = context.Genres.FirstOrDefault(g => g.Name == "Ação");
            var scifi = context.Genres.FirstOrDefault(g => g.Name == "Ficção Científica");
            var drama = context.Genres.FirstOrDefault(g => g.Name == "Drama");
            var adventure = context.Genres.FirstOrDefault(g => g.Name == "Aventura");
            var suspense = context.Genres.FirstOrDefault(g => g.Name == "Suspense");

            var rating14 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "14");
            var rating12 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "12");
            var rating16 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "16");
            var rating18 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "18");
            var rating10 = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "10");
            var ratingLivre = context.Set<AgeRating>().FirstOrDefault(r => r.Rating == "Livre");

            void AddMovieIfNotExists(string title, string synopsis, int duration, string imageUrl, AgeRating? ageRating, List<Genre> genres)
            {
                if (!context.Movies.Any(m => m.Title == title))
                {
                    context.Movies.Add(new Movie
                    {
                        Title = title,
                        Synopsis = synopsis,
                        DurationInMinutes = duration,
                        ImageURL = imageUrl,
                        AgeRating = ageRating,
                        Genres = genres
                    });
                }
            }

            if (action != null && scifi != null)
                AddMovieIfNotExists("A Origem", "Dom Cobb é um ladrão habilidoso, o melhor absoluto na perigosa arte da extração, roubando segredos valiosos das profundezas do subconsciente durante o estado de sonho.", 148, "/uploads/a-origem.png", rating14, new List<Genre> { action, scifi });

            if (action != null)
                AddMovieIfNotExists("Os Vingadores", "Os heróis mais poderosos da Terra, incluindo Homem de Ferro, Capitão América, Thor e Hulk, se unem para combater um inimigo inesperado.", 143, "/uploads/os-vingadores.png", rating12, new List<Genre> { action });

            if (drama != null)
                AddMovieIfNotExists("O Poderoso Chefão", "O patriarca idoso de uma dinastia do crime organizado transfere o controle de seu império clandestino para seu filho relutante.", 175, "/uploads/o-poderoso-chefao.png", rating14, new List<Genre> { drama });

            if (drama != null)
                AddMovieIfNotExists("Clube da Luta", "Um funcionário de escritório insone e um fabricante de sabão formam um clube de luta clandestino que evolui para algo muito mais.", 139, "/uploads/Fight-Club.jpg", rating18, new List<Genre> { drama });

            if (action != null && scifi != null)
                AddMovieIfNotExists("Mad Max: Estrada da Fúria", "Em um mundo pós-apocalíptico, Max se une a Furiosa para escapar de um tirano e sua armada em uma guerra pela sobrevivência.", 120, "/uploads/MadMax.jpg", rating16, new List<Genre> { action, scifi });

            if (action != null)
                AddMovieIfNotExists("Rambo: Programado Para Matar", "Um veterano de guerra é perseguido por um xerife local, desencadeando uma batalha de sobrevivência nas montanhas.", 93, "/uploads/Rambo-First-Blood.jpg", rating16, new List<Genre> { action });

            if (action != null)
                AddMovieIfNotExists("John Wick: De Volta ao Jogo", "Um ex-assassino sai da aposentadoria para rastrear os gângsters que mataram seu cachorro e roubaram seu carro.", 101, "/uploads/john-wick.jpg", rating16, new List<Genre> { action });

            if (action != null && suspense != null)
                AddMovieIfNotExists("Watchmen: O Filme", "Em uma realidade alternativa dos anos 80, super-heróis aposentados investigam o assassinato de um deles enquanto descobrem uma conspiração mortal.", 162, "/uploads/watchmen.jpg", rating16, new List<Genre> { action, suspense });

            // Novos filmes para semear
            if (action != null && scifi != null)
                AddMovieIfNotExists("Matrix", "Um programador de computador descobre que a realidade em que vive é na verdade uma simulação criada por máquinas inteligentes.", 136, "/uploads/matrix.jpg", rating14, new List<Genre> { action, scifi });

            if (adventure != null && scifi != null && drama != null)
                AddMovieIfNotExists("Interestelar", "Uma equipe de exploradores viaja através de um buraco de minhoca no espaço em uma tentativa de garantir a sobrevivência da humanidade.", 169, "/uploads/interstellar.jpg", rating10 ?? ratingLivre, new List<Genre> { adventure, scifi, drama });

            // Filmes que já estavam no banco manualmente (agora formalmente no seed)
            if (drama != null)
                AddMovieIfNotExists("Nocaute", "O campeão de boxe Billy Hope tenta reconstruir sua vida e sua carreira após uma tragédia pessoal devastadora.", 124, "/uploads/e90de3df-4203-46c0-a555-735f6a716baf.jpg", rating14, new List<Genre> { drama });

            if (action != null && adventure != null && drama != null)
                AddMovieIfNotExists("Gran Turismo: De Jogador a Corredor", "Baseado na história real de Jann Mardenborough, o filme acompanha a trajetória de um jovem jogador de videogame que se torna piloto profissional de corrida.", 134, "/uploads/00b8366e-1db9-437b-8506-0af4ccba78da.jpg", rating12, new List<Genre> { action, adventure, drama });

            context.SaveChanges();

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

            // Seed Sessões (Para hoje e amanhã se não houverem sessões futuras)
            if (!context.Sessions.Any(s => s.StartTime > DateTime.Now))
            {
                // Limpar sessões antigas
                var oldSessions = context.Sessions.ToList();
                if (oldSessions.Any())
                {
                    context.Sessions.RemoveRange(oldSessions);
                    context.SaveChanges();
                }

                var movie1 = context.Movies.FirstOrDefault(m => m.Title == "A Origem");
                var movie2 = context.Movies.FirstOrDefault(m => m.Title == "Os Vingadores");
                var movie3 = context.Movies.FirstOrDefault(m => m.Title == "O Poderoso Chefão");
                
                var room1 = context.Rooms.FirstOrDefault(r => r.RoomNumber == 1);
                var room2 = context.Rooms.FirstOrDefault(r => r.RoomNumber == 2);

                if (movie1 != null && room1 != null)
                {
                    // Sessões futuras (a partir de agora)
                    context.Sessions.Add(new Session { Movie = movie1, Room = room1, StartTime = DateTime.Now.AddHours(1), Price = 25.00m });
                    context.Sessions.Add(new Session { Movie = movie1, Room = room1, StartTime = DateTime.Now.AddHours(4), Price = 28.00m });
                    context.Sessions.Add(new Session { Movie = movie1, Room = room1, StartTime = DateTime.Now.AddDays(1).AddHours(2), Price = 30.00m });
                }

                if (movie2 != null && room2 != null)
                {
                    context.Sessions.Add(new Session { Movie = movie2, Room = room2, StartTime = DateTime.Now.AddHours(2), Price = 25.00m });
                    context.Sessions.Add(new Session { Movie = movie2, Room = room2, StartTime = DateTime.Now.AddDays(1).AddHours(3), Price = 25.00m });
                }

                 if (movie3 != null && room1 != null)
                {
                    context.Sessions.Add(new Session { Movie = movie3, Room = room1, StartTime = DateTime.Now.AddHours(6), Price = 35.00m });
                }

                // Novos filmes com imagens locais
                var fightClub = context.Movies.FirstOrDefault(m => m.Title == "Clube da Luta");
                var madMax = context.Movies.FirstOrDefault(m => m.Title == "Mad Max: Estrada da Fúria");
                var rambo = context.Movies.FirstOrDefault(m => m.Title == "Rambo: Programado Para Matar");
                var johnWick = context.Movies.FirstOrDefault(m => m.Title == "John Wick: De Volta ao Jogo");
                var watchmen = context.Movies.FirstOrDefault(m => m.Title == "Watchmen: O Filme");
                
                var room3 = context.Rooms.FirstOrDefault(r => r.RoomNumber == 3);
                var room4 = context.Rooms.FirstOrDefault(r => r.RoomNumber == 4);

                if (fightClub != null && room2 != null)
                {
                    context.Sessions.Add(new Session { Movie = fightClub, Room = room2, StartTime = DateTime.Now.AddHours(3), Price = 30.00m });
                    context.Sessions.Add(new Session { Movie = fightClub, Room = room2, StartTime = DateTime.Now.AddDays(1).AddHours(5), Price = 32.00m });
                }

                if (madMax != null && room3 != null)
                {
                    context.Sessions.Add(new Session { Movie = madMax, Room = room3, StartTime = DateTime.Now.AddMinutes(30), Price = 28.00m });
                    context.Sessions.Add(new Session { Movie = madMax, Room = room3, StartTime = DateTime.Now.AddHours(5), Price = 30.00m });
                    context.Sessions.Add(new Session { Movie = madMax, Room = room3, StartTime = DateTime.Now.AddDays(1).AddHours(4), Price = 30.00m });
                }

                if (rambo != null && room1 != null)
                {
                    context.Sessions.Add(new Session { Movie = rambo, Room = room1, StartTime = DateTime.Now.AddHours(2.5), Price = 25.00m });
                    context.Sessions.Add(new Session { Movie = rambo, Room = room1, StartTime = DateTime.Now.AddDays(1).AddHours(1), Price = 25.00m });
                }

                if (johnWick != null && room4 != null)
                {
                    context.Sessions.Add(new Session { Movie = johnWick, Room = room4, StartTime = DateTime.Now.AddMinutes(45), Price = 32.00m });
                    context.Sessions.Add(new Session { Movie = johnWick, Room = room4, StartTime = DateTime.Now.AddHours(4.5), Price = 35.00m });
                    context.Sessions.Add(new Session { Movie = johnWick, Room = room4, StartTime = DateTime.Now.AddDays(1).AddHours(6), Price = 35.00m });
                }

                if (watchmen != null && room2 != null)
                {
                    context.Sessions.Add(new Session { Movie = watchmen, Room = room2, StartTime = DateTime.Now.AddHours(7), Price = 30.00m });
                    context.Sessions.Add(new Session { Movie = watchmen, Room = room2, StartTime = DateTime.Now.AddDays(1).AddHours(3.5), Price = 30.00m });
                }

                // Sessões para os novos filmes
                var matrix = context.Movies.FirstOrDefault(m => m.Title == "Matrix");
                var interstellar = context.Movies.FirstOrDefault(m => m.Title == "Interestelar");
                var nocaute = context.Movies.FirstOrDefault(m => m.Title == "Nocaute");
                var granTurismo = context.Movies.FirstOrDefault(m => m.Title == "Gran Turismo: De Jogador a Corredor");

                if (matrix != null && room3 != null)
                {
                    context.Sessions.Add(new Session { Movie = matrix, Room = room3, StartTime = DateTime.Now.AddHours(2), Price = 20.00m });
                    context.Sessions.Add(new Session { Movie = matrix, Room = room3, StartTime = DateTime.Now.AddDays(1).AddHours(4), Price = 22.00m });
                }

                if (interstellar != null && room4 != null)
                {
                    context.Sessions.Add(new Session { Movie = interstellar, Room = room4, StartTime = DateTime.Now.AddHours(3.5), Price = 25.00m });
                    context.Sessions.Add(new Session { Movie = interstellar, Room = room4, StartTime = DateTime.Now.AddDays(1).AddHours(6), Price = 25.00m });
                }

                if (nocaute != null && room1 != null)
                {
                    context.Sessions.Add(new Session { Movie = nocaute, Room = room1, StartTime = DateTime.Now.AddHours(1.5), Price = 18.00m });
                }

                if (granTurismo != null && room2 != null)
                {
                    context.Sessions.Add(new Session { Movie = granTurismo, Room = room2, StartTime = DateTime.Now.AddHours(5.5), Price = 22.00m });
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