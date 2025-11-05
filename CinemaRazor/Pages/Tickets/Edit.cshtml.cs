using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaRazor.Data;
using CinemaRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CinemaRazor.Pages.Tickets
{
    public class EditModel : PageModel
    {
        private readonly CinemaContext _context;

        public EditModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int TicketId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SessionId { get; set; }

        [BindProperty]
        public int? SelectedSeatId { get; set; }

        public Ticket Ticket { get; private set; }

        public int MovieId { get; private set; }

        public List<SessionOption> MovieSessions { get; private set; } = new();

        public Session Session { get; private set; }

        public List<SeatInfo> SeatLayout { get; private set; } = new();

        public bool HasSessionsForMovie => MovieSessions.Any();

        private int SeatCapacity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? sessionId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Session).ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            Ticket = ticket;
            TicketId = ticket.Id;
            MovieId = ticket.Session.MovieId;
            SelectedSeatId = ticket.SeatId;

            await LoadSeatCapacityAsync();
            await LoadSessionsForMovieAsync(MovieId);

            SessionId = sessionId ?? ticket.SessionId;

            if (SessionId.HasValue && MovieSessions.All(s => s.Id != SessionId.Value))
            {
                SessionId = ticket.SessionId;
            }

            if (SessionId.HasValue)
            {
                await LoadSessionDetailsAsync(SessionId.Value, Ticket.Id);

                if (Session != null && Session.MovieId != MovieId)
                {
                    MovieId = Session.MovieId;
                    await LoadSessionsForMovieAsync(MovieId);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var ticket = await _context.Tickets
                .Include(t => t.Session).ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.Id == TicketId);

            if (ticket == null)
            {
                return NotFound();
            }

            Ticket = ticket;
            MovieId = ticket.Session.MovieId;

            await LoadSeatCapacityAsync();
            await LoadSessionsForMovieAsync(MovieId);

            if (!SessionId.HasValue)
            {
                SessionId = ticket.SessionId;
            }

            await LoadSessionDetailsAsync(SessionId.Value, ticket.Id);

            if (Session == null)
            {
                ModelState.AddModelError(nameof(SessionId), "Выбранный сеанс не найден.");
            }

            SeatInfo seat = null;

            if (!SelectedSeatId.HasValue)
            {
                ModelState.AddModelError(nameof(SelectedSeatId), "Выберите свободное место.");
            }
            else
            {
                seat = SeatLayout.FirstOrDefault(s => s.SeatId == SelectedSeatId.Value);
                if (seat == null)
                {
                    ModelState.AddModelError(nameof(SelectedSeatId), "Выбранное место не найдено.");
                }
                else if (seat.IsOccupied && !(ticket.SessionId == SessionId && seat.SeatId == ticket.SeatId))
                {
                    ModelState.AddModelError(nameof(SelectedSeatId), "Это место уже занято для выбранного сеанса.");
                }
            }

            if (!ModelState.IsValid)
            {
                SelectedSeatId ??= ticket.SeatId;
                return Page();
            }

            ticket.SessionId = Session.Id;
            ticket.SeatId = seat!.SeatId;
            ticket.Price = Session.Price;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Билет обновлён: {Session.Movie?.Title ?? "Сеанс"}, ряд {seat.RowNumber}, место {seat.SeatNumber}.";

            return RedirectToPage("./Index", new { SelectedSessionId = Session.Id, SelectedMovieId = Session.MovieId });
        }

        private async Task LoadSeatCapacityAsync()
        {
            SeatCapacity = await _context.Seats.AsNoTracking().CountAsync();
        }

        private async Task LoadSessionsForMovieAsync(int movieId)
        {
            MovieSessions = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.StartTime)
                .Select(s => new SessionOption
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Price = s.Price,
                    TicketsSold = s.Tickets.Count
                })
                .ToListAsync();

            foreach (var option in MovieSessions)
            {
                var sold = option.TicketsSold;
                if (Ticket != null && option.Id == Ticket.SessionId)
                {
                    sold = Math.Max(0, sold - 1);
                }

                option.SeatsAvailable = SeatCapacity > 0
                    ? Math.Max(0, SeatCapacity - sold)
                    : 0;
            }
        }

        private async Task LoadSessionDetailsAsync(int sessionId, int ticketIdToExclude)
        {
            Session = await _context.Sessions
                .Include(s => s.Movie)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (Session == null)
            {
                SeatLayout = new List<SeatInfo>();
                return;
            }

            SessionId = Session.Id;

            var occupiedSeatIds = new HashSet<int>(await _context.Tickets
                .AsNoTracking()
                .Where(t => t.SessionId == sessionId && t.Id != ticketIdToExclude)
                .Select(t => t.SeatId)
                .ToListAsync());

            var seats = await _context.Seats
                .AsNoTracking()
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();

            SeatLayout = seats.Select(s => new SeatInfo
            {
                SeatId = s.Id,
                RowNumber = s.RowNumber,
                SeatNumber = s.SeatNumber,
                IsOccupied = occupiedSeatIds.Contains(s.Id)
            }).ToList();
        }

        public class SeatInfo
        {
            public int SeatId { get; set; }
            public int RowNumber { get; set; }
            public int SeatNumber { get; set; }
            public bool IsOccupied { get; set; }
        }

        public class SessionOption
        {
            public int Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int Price { get; set; }
            public int TicketsSold { get; set; }
            public int SeatsAvailable { get; set; }
        }
    }
}
