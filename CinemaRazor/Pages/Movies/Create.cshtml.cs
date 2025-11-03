using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics; // 👈 добавляем для логов

namespace CinemaRazor.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Movie Movie { get; set; }

        public IActionResult OnGet()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres.AsNoTracking().ToList(), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // проверим входные данные
            if (!ModelState.IsValid)
            {
                ViewData["GenreId"] = new SelectList(_context.Genres.AsNoTracking().ToList(), "Id", "Name");
                return Page();
            }

            // проверка жанра
            if (Movie.GenreId == 0)
            {
                ModelState.AddModelError("Movie.GenreId", "Выберите жанр.");
                ViewData["GenreId"] = new SelectList(_context.Genres.AsNoTracking().ToList(), "Id", "Name");
                return Page();
            }

            // 🔍 добавляем отладочную информацию
            Debug.WriteLine($"[DEBUG] Создаётся фильм:");
            Debug.WriteLine($"Title: {Movie.Title}");
            Debug.WriteLine($"GenreId: {Movie.GenreId}");
            Debug.WriteLine($"ReleaseDate: {Movie.ReleaseDate}");
            Debug.WriteLine($"Genres в БД: {string.Join(", ", _context.Genres.Select(g => $"{g.Id}:{g.Name}"))}");

            // убедимся, что жанр существует
            var genreExists = _context.Genres.Any(g => g.Id == Movie.GenreId);
            if (!genreExists)
            {
                ModelState.AddModelError("Movie.GenreId", $"Жанр с Id={Movie.GenreId} не существует!");
                ViewData["GenreId"] = new SelectList(_context.Genres.AsNoTracking().ToList(), "Id", "Name");
                return Page();
            }

            _context.Movies.Add(Movie);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
