using System.Threading.Tasks;
using CinemaRazor.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CinemaRazor.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public DashboardStats Stats { get; private set; } = new();

        public async Task OnGetAsync()
        {
            Stats = new DashboardStats
            {
                Movies = await _context.Movies.CountAsync(),
                Genres = await _context.Genres.CountAsync(),
                Sessions = await _context.Sessions.CountAsync(),
                Employees = await _context.Employees.CountAsync(),
                Positions = await _context.Positions.CountAsync()
            };
        }

        public class DashboardStats
        {
            public int Movies { get; set; }
            public int Genres { get; set; }
            public int Sessions { get; set; }
            public int Employees { get; set; }
            public int Positions { get; set; }
        }
    }
}
