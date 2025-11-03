using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Positions
{
    public class DetailsModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public DetailsModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        public Position Position { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }
            else
            {
                Position = position;
            }
            return Page();
        }
    }
}
