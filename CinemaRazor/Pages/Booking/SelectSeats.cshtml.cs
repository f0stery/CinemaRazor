using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Booking
{
    public class SelectSeatsModel : PageModel
    {
        private readonly CinemaContext _context;

        public SelectSeatsModel(CinemaContext context)
        {
            _context = context;
        }

        public Session Session { get; set; }
        public List<Seat> AllSeats { get; set; }
        public HashSet<int> OccupiedSeatIds { get; set; }
        public int MaxRow { get; set; }
        public int MaxSeatInRow { get; set; }

        [BindProperty]
        public int SessionId { get; set; }

        [BindProperty]
        public int SeatId { get; set; }

        public async Task<IActionResult> OnGetAsync(int sessionId)
        {
            Session = await _context.Sessions
                .Include(s => s.Movie)
                    .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (Session == null)
            {
                return NotFound();
            }

            // Получаем все места
            AllSeats = await _context.Seats
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();

            // Получаем занятые места для этого сеанса (через проданные билеты)
            var occupiedSeats = await _context.Tickets
                .Where(t => t.SessionId == sessionId)
                .Select(t => t.SeatId)
                .ToListAsync();

            OccupiedSeatIds = new HashSet<int>(occupiedSeats);

            // Определяем размеры зала
            if (AllSeats.Any())
            {
                MaxRow = AllSeats.Max(s => s.RowNumber);
                MaxSeatInRow = AllSeats.Max(s => s.SeatNumber);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(SessionId);
            }

            // Проверяем, что место еще не занято
            var existingTicket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.SessionId == SessionId && t.SeatId == SeatId);

            if (existingTicket != null)
            {
                ModelState.AddModelError(string.Empty, "Это место уже занято. Пожалуйста, выберите другое.");
                return await OnGetAsync(SessionId);
            }

            // Получаем цену сеанса
            var session = await _context.Sessions.FindAsync(SessionId);
            if (session == null)
            {
                return NotFound();
            }

            // Создаем билет
            var ticket = new Ticket
            {
                SessionId = SessionId,
                SeatId = SeatId,
                Price = session.Price,
                PurchaseDate = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Confirmation", new { ticketId = ticket.Id });
        }
    }
}
