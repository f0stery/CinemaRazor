using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaRazor.Pages.Sessions
{
    public class EditModel : PageModel
    {
        private readonly CinemaContext _context;

        public EditModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Session Session { get; set; } = default!;

        public List<SelectListItem> MovieOptions { get; private set; } = new();

        public Dictionary<int, int> MovieDurations { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var session = await _context.Sessions
                .Include(s => s.Movie)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (session == null)
                return NotFound();

            Session = session;

            await LoadMoviesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadMoviesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var sessionToUpdate = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == Session.Id);
            if (sessionToUpdate == null)
            {
                return NotFound();
            }

            sessionToUpdate.MovieId = Session.MovieId;
            sessionToUpdate.StartTime = Session.StartTime;
            sessionToUpdate.Price = Session.Price;

            var movie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == Session.MovieId);
            if (movie == null)
            {
                ModelState.AddModelError("Session.MovieId", "Выбранный фильм не найден.");
                return Page();
            }

            sessionToUpdate.EndTime = sessionToUpdate.StartTime.AddMinutes(movie.DurationMinutes);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Sessions.Any(e => e.Id == Session.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadMoviesAsync()
        {
            var movies = await _context.Movies
                .AsNoTracking()
                .OrderBy(m => m.Title)
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.DurationMinutes
                })
                .ToListAsync();

            MovieOptions = movies
                .Select(m => new SelectListItem(m.Title, m.Id.ToString()))
                .ToList();

            MovieDurations = movies.ToDictionary(m => m.Id, m => m.DurationMinutes);
        }
    }
}
