using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public IList<Ticket> Tickets { get; private set; } = new List<Ticket>();

        public SelectList MovieOptions { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedMovieId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedSessionId { get; set; }

        public IList<SessionSummary> SessionSummaries { get; private set; } = new List<SessionSummary>();

        public TicketOverview Overview { get; private set; }

        public int TotalSeats { get; private set; }

        public async Task OnGetAsync()
        {
            var movies = await _context.Movies
                .AsNoTracking()
                .OrderBy(m => m.Title)
                .ToListAsync();

            MovieOptions = new SelectList(movies, "Id", "Title", SelectedMovieId);

            TotalSeats = await _context.Seats.AsNoTracking().CountAsync();

            var ticketsQuery = _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .AsNoTracking()
                .AsQueryable();

            var sessionsQuery = _context.Sessions
                .AsNoTracking()
                .Include(s => s.Movie)
                .Include(s => s.Tickets)
                .AsQueryable();

            if (SelectedMovieId.HasValue)
            {
                ticketsQuery = ticketsQuery.Where(t => t.Session.MovieId == SelectedMovieId.Value);
                sessionsQuery = sessionsQuery.Where(s => s.MovieId == SelectedMovieId.Value);
            }

            SessionSummaries = await sessionsQuery
                .OrderBy(s => s.StartTime)
                .Select(s => new SessionSummary
                {
                    SessionId = s.Id,
                    MovieId = s.MovieId,
                    MovieTitle = s.Movie.Title,
                    StartTime = s.StartTime,
                    Price = s.Price,
                    TicketsSold = s.Tickets.Count,
                    TotalSeats = TotalSeats,
                    Revenue = s.Tickets.Sum(t => (int?)t.Price) ?? 0
                })
                .ToListAsync();

            if (SelectedSessionId.HasValue && SessionSummaries.All(s => s.SessionId != SelectedSessionId.Value))
            {
                SelectedSessionId = null;
            }

            if (SelectedSessionId.HasValue)
            {
                ticketsQuery = ticketsQuery.Where(t => t.SessionId == SelectedSessionId.Value);
            }

            Tickets = await ticketsQuery
                .OrderBy(t => t.Session.StartTime)
                .ThenBy(t => t.Seat.RowNumber)
                .ThenBy(t => t.Seat.SeatNumber)
                .ToListAsync();

            Overview = BuildOverview(movies, SessionSummaries);
        }

        private TicketOverview BuildOverview(IList<Movie> movies, IList<SessionSummary> sessionSummaries)
        {
            if (SelectedSessionId.HasValue)
            {
                var session = sessionSummaries.FirstOrDefault(s => s.SessionId == SelectedSessionId.Value);
                if (session != null)
                {
                    return new TicketOverview
                    {
                        Title = session.MovieTitle,
                        Subtitle = session.StartTime.ToString("dd.MM.yyyy HH:mm"),
                        TicketsSold = session.TicketsSold,
                        SeatsAvailable = session.SeatsAvailable,
                        Revenue = session.Revenue,
                        SessionsCount = 1
                    };
                }
            }

            IEnumerable<SessionSummary> filteredSessions = sessionSummaries;
            string title = "Все билеты";

            if (SelectedMovieId.HasValue)
            {
                filteredSessions = sessionSummaries.Where(s => s.MovieId == SelectedMovieId.Value);
                title = movies.FirstOrDefault(m => m.Id == SelectedMovieId.Value)?.Title ?? "Выбранный фильм";
            }

            var filteredList = filteredSessions.ToList();

            return new TicketOverview
            {
                Title = title,
                Subtitle = $"Сеансов: {filteredList.Count}",
                  TicketsSold = filteredList.Sum(s => s.TicketsSold),
                  SeatsAvailable = filteredList.Sum(s => s.SeatsAvailable),
                  Revenue = filteredList.Sum(s => s.Revenue),
                SessionsCount = filteredList.Count
            };
        }

        public class SessionSummary
        {
            public int SessionId { get; set; }
            public int MovieId { get; set; }
            public string MovieTitle { get; set; }
            public DateTime StartTime { get; set; }
            public int Price { get; set; }
            public int TicketsSold { get; set; }
            public int TotalSeats { get; set; }
            public int Revenue { get; set; }
            public int SeatsAvailable => Math.Max(0, TotalSeats - TicketsSold);
            public string DisplayLabel => $"{MovieTitle} — {StartTime:dd.MM.yyyy HH:mm}";
        }

        public class TicketOverview
        {
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public int TicketsSold { get; set; }
            public int SeatsAvailable { get; set; }
            public int Revenue { get; set; }
            public int SessionsCount { get; set; }
        }
    }
}
