using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Positions
{
    public class CreateModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public CreateModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Position Position { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Positions.Add(Position);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Должность '{Position.Title}' успешно создана.";

            return RedirectToPage("./Index");
        }
    }
}
