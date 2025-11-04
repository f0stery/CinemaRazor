using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Sessions
{
    public class BookModel : PageModel
    {
        private readonly CinemaContext _context;

        public BookModel(CinemaContext context)
        {
            _context = context;
        }

        public Session Session { get; set; }
        public List<SeatInfo> SeatLayout { get; set; } = new List<SeatInfo>();
        public HashSet<int> OccupiedSeatIds { get; set; } = new HashSet<int>();

        [BindProperty(SupportsGet = true)]
        public int? SessionId { get; set; }

        [BindProperty]
        public int? SelectedSeatId { get; set; }

        public SelectList SessionOptions { get; set; }

        public async Task OnGetAsync()
        {
            await LoadSessionsAsync();
            
            if (SessionId.HasValue)
            {
                await LoadSessionDetailsAsync(SessionId.Value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!SelectedSeatId.HasValue || !SessionId.HasValue)
            {
                ModelState.AddModelError("", "Выберите место для бронирования.");
                await LoadSessionsAsync();
                if (SessionId.HasValue)
                {
                    await LoadSessionDetailsAsync(SessionId.Value);
                }
                return Page();
            }

            // Проверяем, что место существует
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.Id == SelectedSeatId.Value);

            if (seat == null)
            {
                ModelState.AddModelError("", "Место не найдено.");
                await LoadSessionsAsync();
                if (SessionId.HasValue)
                {
                    await LoadSessionDetailsAsync(SessionId.Value);
                }
                return Page();
            }

            // Проверяем, что место не занято (проверяем наличие билета)
            var ticketExists = await _context.Tickets
                .AnyAsync(t => t.SessionId == SessionId.Value && t.SeatId == SelectedSeatId.Value);

            if (ticketExists)
            {
                ModelState.AddModelError("", "Это место уже забронировано.");
                await LoadSessionsAsync();
                await LoadSessionDetailsAsync(SessionId.Value);
                return Page();
            }

            // Создаем билет
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == SessionId.Value);
            var ticket = new Ticket
            {
                SessionId = SessionId.Value,
                SeatId = SelectedSeatId.Value,
                Price = session?.Price ?? 0,
                PurchaseDate = System.DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Место успешно забронировано! Ряд {seat.RowNumber}, Место {seat.SeatNumber}";
            return RedirectToPage("./Book", new { SessionId = SessionId.Value });
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
                    Display = $"{s.Movie.Title} - {s.StartTime:dd.MM.yyyy HH:mm}"
                })
                .ToListAsync();

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
                return;
            }

            // Загружаем все места зала
            var seats = await _context.Seats
                .AsNoTracking()
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();

            // Загружаем занятые места (проверяем наличие билетов)
            var occupiedSeats = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.SessionId == sessionId)
                .Select(t => t.SeatId)
                .ToListAsync();

            OccupiedSeatIds = new HashSet<int>(occupiedSeats);

            // Создаем макет зала
            SeatLayout = seats.Select(s => new SeatInfo
            {
                SeatId = s.Id,
                RowNumber = s.RowNumber,
                SeatNumber = s.SeatNumber,
                IsOccupied = OccupiedSeatIds.Contains(s.Id)
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
