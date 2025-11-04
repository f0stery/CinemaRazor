using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Booking
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public IList<Session> Sessions { get; set; } = new List<Session>();

        public async Task OnGetAsync()
        {
            // Получаем только будущие сеансы
            Sessions = await _context.Sessions
                .Include(s => s.Movie)
                    .ThenInclude(m => m.Genre)
                .Where(s => s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
