using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Pages
{
    public class UnitActivationsModel : PageModel
    {
        private readonly SourceDataDbContext _context;
        public List<UnitUsageDataModel> unitUsageData { get; set; }

        public UnitActivationsModel(SourceDataDbContext context)
        {
            _context = context;
            unitUsageData = context.unitUsage
                .Include(i => i.DateInterval)
                .Include(i => i.ActivationPercentages)
                    .ThenInclude(a => a.Unit)
                .ToList();
        }

        public void OnGet()
        {
        }
    }
}
