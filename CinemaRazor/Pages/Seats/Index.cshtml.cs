using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Seats
{
    public class IndexModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public IndexModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        public IList<Seat> Seats { get; private set; } = new List<Seat>();

        public async Task OnGetAsync()
        {
            Seats = await _context.Seats
                .AsNoTracking()
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }
    }
}
