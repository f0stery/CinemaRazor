using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Sessions
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public IList<Session> Session { get; set; } = new List<Session>();

        public async Task OnGetAsync()
        {
            Session = await _context.Sessions
                .Include(s => s.Movie) // Только фильм, без зала
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
