using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Seats
{
    public class EditModel : PageModel
    {
        private readonly CinemaContext _context;

        public EditModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seat Seat { get; set; } = default!;

        public SelectList SessionOptions { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Seat = await _context.Seats.FirstOrDefaultAsync(m => m.Id == id);

            if (Seat == null)
                return NotFound();

            await LoadSessionsAsync(Seat.SessionId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSessionsAsync(Seat.SessionId);
                return Page();
            }
                     
            _context.Attach(Seat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeatExists(Seat.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool SeatExists(int id)
        {
            return _context.Seats.Any(e => e.Id == id);
        }

        private async Task LoadSessionsAsync(int? selectedSessionId = null)
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

            SessionOptions = new SelectList(sessions, "Id", "Display", selectedSessionId);
        }
    }
}
