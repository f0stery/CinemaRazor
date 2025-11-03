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
    public class IndexModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public IndexModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        public IList<Ticket> Ticket { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Ticket = await _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Session).ToListAsync();
        }
    }
}
