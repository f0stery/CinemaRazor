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
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Session Session { get; set; } = new Session();

        public List<SelectListItem> MovieOptions { get; private set; } = new();

        public Dictionary<int, int> MovieDurations { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadMoviesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadMoviesAsync();

            // Проверяем, что Session был правильно привязан
            if (Session == null)
            {
                ModelState.AddModelError(string.Empty, "Ошибка при обработке данных формы.");
                return Page();
            }

            if (Session.MovieId == 0)
            {
                ModelState.AddModelError("Session.MovieId", "Выберите фильм.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == Session.MovieId);

            if (movie == null)
            {
                ModelState.AddModelError("Session.MovieId", "Выбранный фильм не найден.");
                return Page();
            }

            if (Session.StartTime == default)
            {
                ModelState.AddModelError("Session.StartTime", "Укажите дату и время начала сеанса.");
                return Page();
            }

            Session.EndTime = Session.StartTime.AddMinutes(movie.DurationMinutes);

            _context.Sessions.Add(Session);
            await _context.SaveChangesAsync();

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
