using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System.Threading.Tasks;
using System.Linq;

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

        public IActionResult OnGet()
        {
            // Загружаем список фильмов (залы не нужны, так как их только один)
            ViewData["MovieId"] = new SelectList(_context.Movies.AsNoTracking().ToList(), "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Проверяем, что Session был правильно привязан
            if (Session == null)
            {
                ModelState.AddModelError("", "Ошибка при обработке данных формы.");
                ViewData["MovieId"] = new SelectList(_context.Movies.AsNoTracking().ToList(), "Id", "Title");
                return Page();
            }

            // Проверяем, что MovieId выбран
            if (Session.MovieId == 0)
            {
                ModelState.AddModelError("Session.MovieId", "Выберите фильм.");
            }
            else
            {
                // Проверяем, что фильм существует
                var movieExists = await _context.Movies.AnyAsync(m => m.Id == Session.MovieId);
                if (!movieExists)
                {
                    ModelState.AddModelError("Session.MovieId", $"Фильм с Id={Session.MovieId} не существует!");
                }
            }

            // Проверяем валидность модели
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = new SelectList(_context.Movies.AsNoTracking().ToList(), "Id", "Title");
                return Page();
            }

            // 💾 Добавляем сеанс и сохраняем изменения
            _context.Sessions.Add(Session);
            await _context.SaveChangesAsync();

            // 🔙 Возврат на страницу списка
            return RedirectToPage("./Index");
        }
    }
}
