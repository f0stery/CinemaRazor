using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaRazor.Data;

namespace CinemaRazor.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly CinemaRazor.Data.CinemaContext _context;

        public IndexModel(CinemaRazor.Data.CinemaContext context)
        {
            _context = context;
        }

        public IList<EmployeeListItem> Employee { get; private set; } = new List<EmployeeListItem>();

        public async Task OnGetAsync()
        {
            Employee = await _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.FullName)
                .Select(e => new EmployeeListItem
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Age = e.Age,
                    Gender = e.Gender,
                    Address = e.Address,
                    Phone = e.Phone,
                    PositionTitle = e.Position != null ? e.Position.Title : null
                })
                .ToListAsync();
        }

        public class EmployeeListItem
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public int Age { get; set; }
            public string? Gender { get; set; }
            public string? Address { get; set; }
            public string? Phone { get; set; }
            public string? PositionTitle { get; set; }
        }
    }
}
