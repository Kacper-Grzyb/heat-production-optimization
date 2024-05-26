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

            productionUnits = new();
            List<OptimizerUnitNamesDataModel> productionUnitNames = _context.productionUnitNamesForOptimization.ToList();

            foreach(var record in productionUnitNames)
            {
				var boiler = _context.productionUnits.FirstOrDefault(u => u.Name.ToLower() == record.Name.ToLower());
                if(boiler != null)
                {
                    productionUnits.Add(boiler);
                }
			}
        }
    }
}
