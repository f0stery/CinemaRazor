using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Booking
{
    public class ConfirmationModel : PageModel
    {
        private readonly CinemaContext _context;

        public ConfirmationModel(CinemaContext context)
        {
            _context = context;
        }

        public Ticket Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int ticketId)
        {
            Ticket = await _context.Tickets
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                        .ThenInclude(m => m.Genre)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (Ticket == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
