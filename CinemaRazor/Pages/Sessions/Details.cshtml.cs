using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Sessions
{
    public class DetailsModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public DetailsModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        public Session Session { get; set; } = default!;
        public List<SeatInfo> SeatLayout { get; set; } = new List<SeatInfo>();
        public HashSet<int> OccupiedSeatIds { get; set; } = new HashSet<int>();
        public int TotalSeats { get; set; }
        public int OccupiedSeats { get; set; }
        public int AvailableSeats { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (session == null)
            {
                return NotFound();
            }

            Session = session;

            // Загружаем места для этого сеанса
            var seats = await _context.Seats
                .AsNoTracking()
                .Where(s => s.SessionId == id.Value)
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();

            // Загружаем занятые места
            var occupiedSeats = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.SessionId == id.Value)
                .Select(t => t.SeatId)
                .ToListAsync();

            OccupiedSeatIds = new HashSet<int>(occupiedSeats);
            TotalSeats = seats.Count;
            OccupiedSeats = occupiedSeats.Count;
            AvailableSeats = TotalSeats - OccupiedSeats;

            SeatLayout = seats.Select(s => new SeatInfo
            {
                SeatId = s.Id,
                RowNumber = s.RowNumber,
                SeatNumber = s.SeatNumber,
                IsOccupied = OccupiedSeatIds.Contains(s.Id)
            }).ToList();

            return Page();
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
