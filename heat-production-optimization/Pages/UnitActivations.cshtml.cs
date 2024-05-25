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
        public List<ProductionUnitDataModel> productionUnits { get; set; }

        public UnitActivationsModel(SourceDataDbContext context)
        {
            _context = context;
            unitUsageData = _context.unitUsage
                .Include(i => i.DateInterval)
                .Include(i => i.ActivationPercentages)
                    .ThenInclude(a => a.Unit)
                .ToList();
            unitUsageData = unitUsageData.OrderBy(unit => unit.DateInterval.TimeFrom).ToList();

            productionUnits = _context.productionUnits.ToList();

            // There is an error here if you do these actions:
            // 1) You optimize the data
            // 2) You change the optimization to only use one boiler
            // 3) You click optimize
            // 4) You change some of the properties for that boiler
            // 5) You re-optimize
            // 6) You go to the unit activations page
        }

        public void OnGet()
        {
        }
    }
}
