using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Pages.Shared
{
    public class _DataUploadModel : PageModel
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();

        public _DataUploadModel(SourceDataDbContext context)
        {
            if (context != null) _context = context;
        }

        public void OnGet()
        {

        }

        
    }
}
