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

        public IList<TicketSalesInfo> SalesSummary { get; private set; } = new List<TicketSalesInfo>();

        public SelectList MovieOptions { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedMovieId { get; set; }

        public int? SelectedMovieTicketsSold => SelectedMovieId.HasValue
            ? SalesSummary.FirstOrDefault(s => s.MovieId == SelectedMovieId.Value)?.TicketsSold
            : null;

        public async Task OnGetAsync()
        {
            var movies = await _context.Movies
                .AsNoTracking()
                .OrderBy(m => m.Title)
                .ToListAsync();

            MovieOptions = new SelectList(movies, "Id", "Title");

            var ticketsQuery = _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .AsNoTracking()
                .AsQueryable();

            if (SelectedMovieId.HasValue)
            {
                ticketsQuery = ticketsQuery.Where(t => t.Session.MovieId == SelectedMovieId.Value);
            }

            Tickets = await ticketsQuery
                .OrderBy(t => t.Session.StartTime)
                .ThenBy(t => t.Seat.RowNumber)
                .ThenBy(t => t.Seat.SeatNumber)
                .ToListAsync();

            SalesSummary = await _context.Tickets
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .AsNoTracking()
                .GroupBy(t => new { t.Session.MovieId, t.Session.Movie.Title })
                .Select(g => new TicketSalesInfo
                {
                    MovieId = g.Key.MovieId,
                    MovieTitle = g.Key.Title,
                    TicketsSold = g.Count()
                })
                .OrderByDescending(s => s.TicketsSold)
                .ThenBy(s => s.MovieTitle)
                .ToListAsync();
        }

        public class TicketSalesInfo
        {
            public int MovieId { get; set; }
            public string MovieTitle { get; set; }
            public int TicketsSold { get; set; }
        }
    }
}
