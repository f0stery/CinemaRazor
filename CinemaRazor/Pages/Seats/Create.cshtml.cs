using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CinemaRazor.Data;
using CinemaRazor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaRazor.Pages.Seats
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seat Seat { get; set; }

        public SelectList SessionOptions { get; private set; }

        public async Task OnGetAsync()
        {
            await LoadSessionsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSessionsAsync();
                return Page();
            }

            _context.Seats.Add(Seat);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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

            SessionOptions = new SelectList(sessions, "Id", "Display");
        }
    }
}
