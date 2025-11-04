using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaRazor.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CinemaRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public DashboardStats Stats { get; private set; } = new();

        public IList<SessionSummary> UpcomingSessions { get; private set; } = new List<SessionSummary>();

        public IList<TicketSalesSummary> TicketSales { get; private set; } = new List<TicketSalesSummary>();

        public async Task OnGetAsync()
        {
            Stats = new DashboardStats
            {
                Movies = await _context.Movies.CountAsync(),
                Genres = await _context.Genres.CountAsync(),
                Sessions = await _context.Sessions.CountAsync(),
                Tickets = await _context.Tickets.CountAsync(),
                Employees = await _context.Employees.CountAsync(),
                Positions = await _context.Positions.CountAsync()
            };

            var now = DateTime.Now;

            UpcomingSessions = await _context.Sessions
                .AsNoTracking()
                .Include(s => s.Movie)
                .Where(s => s.StartTime >= now)
                .OrderBy(s => s.StartTime)
                .Take(5)
                .Select(s => new SessionSummary
                {
                    SessionId = s.Id,
                    MovieTitle = s.Movie.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    DurationMinutes = s.Movie.DurationMinutes,
                    Price = s.Price
                })
                .ToListAsync();

            TicketSales = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .GroupBy(t => new { t.Session.MovieId, t.Session.Movie.Title })
                .Select(g => new TicketSalesSummary
                {
                    MovieId = g.Key.MovieId,
                    MovieTitle = g.Key.Title,
                    TicketsSold = g.Count()
                })
                .OrderByDescending(x => x.TicketsSold)
                .ThenBy(x => x.MovieTitle)
                .Take(5)
                .ToListAsync();
        }

        public class DashboardStats
        {
            public int Movies { get; set; }
            public int Genres { get; set; }
            public int Sessions { get; set; }
            public int Tickets { get; set; }
            public int Employees { get; set; }
            public int Positions { get; set; }
        }

        public class SessionSummary
        {
            public int SessionId { get; set; }
            public string MovieTitle { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int DurationMinutes { get; set; }
            public decimal Price { get; set; }
        }

        public class TicketSalesSummary
        {
            public int MovieId { get; set; }
            public string MovieTitle { get; set; }
            public int TicketsSold { get; set; }
        }
    }
}
