using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CinemaRazor.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public IList<Movie> Movies { get; set; }

        public async Task OnGetAsync()
        {
            // Загружаем фильмы вместе с жанрами
            Movies = await _context.Movies
                .Include(m => m.Genre)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
