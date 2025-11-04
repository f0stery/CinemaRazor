using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Seats
{
    public class EditModel : PageModel
    {
        private readonly CinemaContext _context;

        public EditModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seat Seat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Seat = await _context.Seats.FirstOrDefaultAsync(m => m.Id == id);

            if (Seat == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
                     
            _context.Attach(Seat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeatExists(Seat.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool SeatExists(int id)
        {
            return _context.Seats.Any(e => e.Id == id);
        }
    }
}
