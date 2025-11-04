using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System.Threading.Tasks;

namespace CinemaRazor.Pages.Seats
{
    public class CreateModel : PageModel
    {
        private readonly CinemaContext _context;

        public CreateModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seat Seat { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Seats.Add(Seat);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
