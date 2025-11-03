using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Sessions
{
    public class EditModel : PageModel
    {
        private readonly CinemaContext _context;

        public EditModel(CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Session Session { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var session = await _context.Sessions
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (session == null)
                return NotFound();

            Session = session;

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
                return Page();
            }
                        
            _context.Attach(Session).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Sessions.Any(e => e.Id == Session.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
