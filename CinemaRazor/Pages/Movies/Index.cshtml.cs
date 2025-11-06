using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaRazor.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaRazor.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly CinemaContext _context;

        public IndexModel(CinemaContext context)
        {
            _context = context;
        }

        public IList<MovieListItem> Movies { get; private set; } = new List<MovieListItem>();

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
                .AsNoTracking()
                .AsQueryable();

            if (SelectedGenreId.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.GenreId == SelectedGenreId.Value);
            }

            Movies = await moviesQuery
                .OrderBy(m => m.Title)
                .Select(m => new MovieListItem
                {
                    Id = m.Id,
                    Title = m.Title,
                    GenreName = m.Genre != null ? m.Genre.Name : null,
                    ProducerCompany = m.ProducerCompany,
                    ProductionCountry = m.ProductionCountry,
                    AgeRating = m.AgeRating,
                    ReleaseDate = m.ReleaseDate,
                    DurationMinutes = m.DurationMinutes,
                    Actors = m.Actors
                })
                .ToListAsync();
        }

        public class MovieListItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? GenreName { get; set; }
            public string? ProducerCompany { get; set; }
            public string? ProductionCountry { get; set; }
            public int? AgeRating { get; set; }
            public DateTime ReleaseDate { get; set; }
            public int DurationMinutes { get; set; }
            public string? Actors { get; set; }
        }
    }
}
