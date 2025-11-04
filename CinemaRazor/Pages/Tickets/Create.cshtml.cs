using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaRazor.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket { PurchaseDate = DateTime.Now };

        public SelectList SessionOptions { get; private set; }

        public SelectList SeatOptions { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSelectionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == Ticket.SessionId);
            if (session == null)
            {
                ModelState.AddModelError("Ticket.SessionId", "Выбранный сеанс не найден.");
            }

            var seat = await _context.Seats.FirstOrDefaultAsync(s => s.Id == Ticket.SeatId);
            if (seat == null)
            {
                ModelState.AddModelError("Ticket.SeatId", "Выбранное место не найдено.");
            }
            else if (seat.SessionId != Ticket.SessionId)
            {
                ModelState.AddModelError("Ticket.SeatId", "Место не относится к выбранному сеансу.");
            }
            else if (await _context.Tickets.AnyAsync(t => t.SessionId == Ticket.SessionId && t.SeatId == Ticket.SeatId))
            {
                ModelState.AddModelError(string.Empty, "Это место уже продано для выбранного сеанса.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectionsAsync(Ticket.SessionId);
                return Page();
            }

            if (Ticket.Price <= 0 && session != null)
            {
                Ticket.Price = session.Price;
            }

            if (Ticket.PurchaseDate == default)
            {
                Ticket.PurchaseDate = DateTime.Now;
            }

            _context.Tickets.Add(Ticket);
            seat!.IsOccupied = true;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private async Task LoadSelectionsAsync(int? selectedSessionId = null)
        {
            var sessionItems = await _context.Sessions
                .Include(s => s.Movie)
                .AsNoTracking()
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    s.Id,
                    Display = $"{s.Movie.Title} ({s.StartTime:dd.MM.yyyy HH:mm})"
                })
                .ToListAsync();

            var resolvedSessionId = selectedSessionId ?? sessionItems.FirstOrDefault()?.Id;

            if (!selectedSessionId.HasValue && resolvedSessionId.HasValue)
            {
                Ticket.SessionId = resolvedSessionId.Value;
            }

            SessionOptions = new SelectList(sessionItems, "Id", "Display", resolvedSessionId);

            var seats = await _context.Seats
                .Include(s => s.Session)
                    .ThenInclude(sess => sess.Movie)
                .AsNoTracking()
                .Where(s => (!s.IsOccupied || s.Id == Ticket.SeatId) && (!resolvedSessionId.HasValue || s.SessionId == resolvedSessionId))
                .OrderBy(s => s.Session.StartTime)
                .ThenBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .Select(s => new
                {
                    s.Id,
                    Display = $"{s.Session.Movie.Title} - {s.Session.StartTime:dd.MM HH:mm}, ряд {s.RowNumber}, место {s.SeatNumber}"
                })
                .ToListAsync();

            SeatOptions = new SelectList(seats, "Id", "Display", Ticket.SeatId == 0 ? null : Ticket.SeatId);
        }
    }
}
