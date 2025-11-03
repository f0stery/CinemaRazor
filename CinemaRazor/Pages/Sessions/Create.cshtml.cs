using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;
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
        public Session Session { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Загружаем список фильмов (залы не нужны, так как их только один)
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
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
