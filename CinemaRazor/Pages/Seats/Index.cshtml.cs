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

        public IList<Seat> Seat { get;set; } = default!;
    }
}
