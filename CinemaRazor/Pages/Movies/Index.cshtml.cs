using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using CinemaRazor.Models;
using Microsoft.AspNetCore.Mvc;
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

        public SelectList GenreOptions { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedGenreId { get; set; }

        public async Task OnGetAsync()
        {
            var genres = await _context.Genres
                .OrderBy(g => g.Name)
                .AsNoTracking()
                .ToListAsync();

            GenreOptions = new SelectList(genres, "Id", "Name");

            var moviesQuery = _context.Movies
                .Include(m => m.Genre)
                .AsNoTracking()
                .AsQueryable();

            if (SelectedGenreId.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.GenreId == SelectedGenreId.Value);
            }

            Movies = await moviesQuery.ToListAsync();
        }
    }
}
