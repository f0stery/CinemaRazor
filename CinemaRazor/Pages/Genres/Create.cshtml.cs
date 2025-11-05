using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System.Threading.Tasks;

namespace CinemaRazor.Pages.Genres
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Genre Genre { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Genres.Add(Genre);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Жанр '{Genre.Name}' создан.";

            return RedirectToPage("./Index");
        }
    }
}
