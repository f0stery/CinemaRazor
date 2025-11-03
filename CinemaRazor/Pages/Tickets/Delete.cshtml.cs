using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Tickets
{
    public class DeleteModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public DeleteModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }
            else
            {
                Ticket = ticket;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                Ticket = ticket;
                _context.Tickets.Remove(Ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
