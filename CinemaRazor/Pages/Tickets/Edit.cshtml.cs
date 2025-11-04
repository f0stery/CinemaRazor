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
    public class EditModel : PageModel
    {
        private readonly CinemaContext _context;

        public EditModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = default!;

        public SelectList SessionOptions { get; private set; }

        public SelectList SeatOptions { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            Ticket = ticket;
            await LoadSelectionsAsync(Ticket.SessionId, Ticket.SeatId);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var ticketFromDb = await _context.Tickets
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.Id == Ticket.Id);

            if (ticketFromDb == null)
            {
                return NotFound();
            }

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
            else if (await _context.Tickets.AnyAsync(t => t.SessionId == Ticket.SessionId && t.SeatId == Ticket.SeatId && t.Id != Ticket.Id))
            {
                ModelState.AddModelError(string.Empty, "Это место уже продано для выбранного сеанса.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectionsAsync(Ticket.SessionId, Ticket.SeatId);
                return Page();
            }

            var previousSeatId = ticketFromDb.SeatId;

            ticketFromDb.SessionId = Ticket.SessionId;
            ticketFromDb.SeatId = Ticket.SeatId;
            ticketFromDb.Price = Ticket.Price <= 0 && session != null ? session.Price : Ticket.Price;
            ticketFromDb.PurchaseDate = Ticket.PurchaseDate == default ? DateTime.Now : Ticket.PurchaseDate;

            if (previousSeatId != Ticket.SeatId)
            {
                var previousSeat = await _context.Seats.FirstOrDefaultAsync(s => s.Id == previousSeatId);
                if (previousSeat != null)
                {
                    previousSeat.IsOccupied = false;
                }
            }

            if (seat != null)
            {
                seat.IsOccupied = true;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(Ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        private async Task LoadSelectionsAsync(int? selectedSessionId = null, int? selectedSeatId = null)
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

            SessionOptions = new SelectList(sessionItems, "Id", "Display", resolvedSessionId);

            var seatsQuery = _context.Seats
                .Include(s => s.Session)
                    .ThenInclude(sess => sess.Movie)
                .AsNoTracking()
                .OrderBy(s => s.Session.StartTime)
                .ThenBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .Select(s => new
                {
                    s.Id,
                    s.IsOccupied,
                    s.SessionId,
                    Display = $"{s.Session.Movie.Title} - {s.Session.StartTime:dd.MM HH:mm}, ряд {s.RowNumber}, место {s.SeatNumber}"
                });

            var seats = await seatsQuery
                .Where(s => (!s.IsOccupied || s.Id == selectedSeatId) && (!resolvedSessionId.HasValue || s.SessionId == resolvedSessionId))
                .ToListAsync();

            SeatOptions = new SelectList(seats, "Id", "Display", selectedSeatId);
        }
    }
}
