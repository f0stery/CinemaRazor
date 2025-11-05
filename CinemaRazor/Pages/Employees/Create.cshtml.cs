using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using CinemaRazor.Models;
using Microsoft.EntityFrameworkCore;

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
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulatePositionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var hasPositions = await PopulatePositionsAsync();
            if (!hasPositions)
            {
                ModelState.AddModelError(string.Empty, "Сначала создайте хотя бы одну должность.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private async Task<bool> PopulatePositionsAsync()
        {
            var positions = await _context.Positions
                .AsNoTracking()
                .OrderBy(p => p.Title)
                .ToListAsync();

            ViewData["PositionId"] = new SelectList(positions, "Id", "Title");
            ViewData["HasPositions"] = positions.Any();
            return positions.Any();
        }
    }
}
