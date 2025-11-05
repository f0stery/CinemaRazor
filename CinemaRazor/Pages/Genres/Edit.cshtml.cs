using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;
using CinemaRazor.Models;

namespace CinemaRazor.Pages.Genres
{
    public class EditModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public EditModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Genre Genre { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre =  await _context.Genres.FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            Genre = genre;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var genreToUpdate = await _context.Genres.FirstOrDefaultAsync(g => g.Id == Genre.Id);
            if (genreToUpdate == null)
            {
                return NotFound();
            }

            genreToUpdate.Name = Genre.Name;
            genreToUpdate.Description = Genre.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Genres.AnyAsync(e => e.Id == Genre.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = $"Жанр '{genreToUpdate.Name}' обновлён.";

            return RedirectToPage("./Index");
        }
    }
}
