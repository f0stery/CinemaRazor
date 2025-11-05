using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Positions
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Position Position { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("=== [DEBUG] OnPostAsync called ===");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState invalid!");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                return Page();
            }

            try
            {
                Console.WriteLine($"Saving position: {Position.Title}");
                _context.Positions.Add(Position);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"✅ Должность '{Position.Title}' успешно создана.";
                Console.WriteLine("✅ Position saved successfully.");

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ Database update failed: {dbEx.InnerException?.Message ?? dbEx.Message}");
                ModelState.AddModelError(string.Empty, "Ошибка при сохранении в базу данных. Проверьте корректность введённых данных.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Произошла непредвиденная ошибка. Повторите попытку позже.");
            }

            // Если что-то пошло не так — возвращаем страницу с ошибкой
            return Page();
        }
    }
}
