using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Pages
{
    public class DataDisplayModel : PageModel
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();
        public DbSet<HeatDemandDataModel>? heatDemandData;

        public DataDisplayModel(SourceDataDbContext context)
        {
            if (context != null) _context = context;
        }
        public void OnGet()
        {
            heatDemandData = _context.HeatDemandData;
		}
    }
}
