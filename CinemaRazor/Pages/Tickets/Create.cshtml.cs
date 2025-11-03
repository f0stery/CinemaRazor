using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = default!;

        public IActionResult OnGet()
        {
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id");
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
