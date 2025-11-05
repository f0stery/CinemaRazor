using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaRazor.Data;
using CinemaRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaRazor.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int? SessionId { get; set; }

        [BindProperty]
        public int? SelectedSeatId { get; set; }

        public SelectList SessionOptions { get; private set; }

        public Session Session { get; private set; }

        public List<SeatInfo> SeatLayout { get; private set; } = new();

        public bool HasSessions => SessionOptions?.Any() == true;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSessionsAsync();

            if (SessionId.HasValue)
            {
                await LoadSessionDetailsAsync(SessionId.Value);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSessionsAsync();

            if (!SessionId.HasValue)
            {
                ModelState.AddModelError(nameof(SessionId), "Выберите сеанс.");
                return Page();
            }

            await LoadSessionDetailsAsync(SessionId.Value);

            if (Session == null)
            {
                ModelState.AddModelError(nameof(SessionId), "Выбранный сеанс не найден.");
                return Page();
            }

            if (!SelectedSeatId.HasValue)
            {
                ModelState.AddModelError(nameof(SelectedSeatId), "Выберите свободное место.");
                return Page();
            }

            var seat = SeatLayout.FirstOrDefault(s => s.SeatId == SelectedSeatId.Value);
            if (seat == null)
            {
                ModelState.AddModelError(nameof(SelectedSeatId), "Выбранное место не найдено.");
                return Page();
            }

            if (seat.IsOccupied)
            {
                ModelState.AddModelError(nameof(SelectedSeatId), "Это место уже продано для выбранного сеанса.");
                return Page();
            }

            var ticket = new Ticket
            {
                SessionId = Session.Id,
                SeatId = seat.SeatId,
                Price = Session.Price,
                PurchaseDate = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Продан билет: {Session.Movie?.Title ?? "Сеанс"}, ряд {seat.RowNumber}, место {seat.SeatNumber}.";

            return RedirectToPage("./Index", new { SelectedSessionId = Session.Id });
        }

        private async Task LoadSessionsAsync()
        {
            var sessions = await _context.Sessions
                .Include(s => s.Movie)
                .AsNoTracking()
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    s.Id,
                    Display = $"{s.Movie.Title} ({s.StartTime:dd.MM.yyyy HH:mm})"
                })
                .ToListAsync();

            if (!SessionId.HasValue && sessions.Any())
            {
                SessionId = sessions.First().Id;
            }

            SessionOptions = new SelectList(sessions, "Id", "Display", SessionId);
        }

        private async Task LoadSessionDetailsAsync(int sessionId)
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

            var occupiedSeatIds = new HashSet<int>(await _context.Tickets
                .AsNoTracking()
                .Where(t => t.SessionId == sessionId)
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
    }
}
