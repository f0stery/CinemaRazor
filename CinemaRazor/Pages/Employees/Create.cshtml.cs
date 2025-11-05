using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        public SelectList? PositionsList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulatePositionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("=== [DEBUG] OnPostAsync (Employee) called ===");

            var hasPositions = await PopulatePositionsAsync();
            if (!hasPositions)
            {
                ModelState.AddModelError(string.Empty, "Сначала создайте хотя бы одну должность.");
                return Page();
            }

            // Проверка — выбрал ли пользователь должность
            if (Employee.PositionId == 0)
            {
                ModelState.AddModelError("Employee.PositionId", "Выберите должность.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState invalid!");
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                        Console.WriteLine($"[VALIDATION ERROR] {kv.Key}: {err.ErrorMessage}");
                }

                await PopulatePositionsAsync(); // повторно загружаем select
                return Page();
            }

            try
            {
                Console.WriteLine($"👤 Добавляется сотрудник: {Employee.FullName}, должность ID = {Employee.PositionId}");
                _context.Employees.Add(Employee);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Сотрудник сохранён успешно.");

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ Database update failed: {dbEx.InnerException?.Message ?? dbEx.Message}");
                ModelState.AddModelError(string.Empty, "Ошибка при сохранении данных в базу.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Непредвиденная ошибка при добавлении сотрудника.");
            }

            await PopulatePositionsAsync();
            return Page();
        }

        private async Task<bool> PopulatePositionsAsync()
        {
            var positions = await _context.Positions
                .AsNoTracking()
                .OrderBy(p => p.Title)
                .ToListAsync();

            PositionsList = new SelectList(positions, "Id", "Title");
            ViewData["PositionId"] = PositionsList;
            ViewData["HasPositions"] = positions.Any();

            Console.WriteLine($"[DEBUG] Найдено должностей: {positions.Count}");
            return positions.Any();
        }
    }
}
