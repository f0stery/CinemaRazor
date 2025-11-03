using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public Employee Employee { get; set; } = default!;

        public IActionResult OnGet()
        {
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
