using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using heat_production_optimization.Models;

namespace heat_production_optimization.Pages
{
    public class ResultDataManagerModel : PageModel
    {
        private readonly SourceDataDbContext _context;
        private readonly Optimizer _optimizer;

        public ResultDataManagerModel(SourceDataDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _optimizer = new Optimizer(context.productionUnits, context.HeatDemandData);
        }

        public double GasProductionCost { get; set; }
        public double GasCO2Emission { get; set; }
        public double OilProductionCost { get; set; }
        public double OilCO2Emission { get; set; }

        public void OnGet()
        {
			double heatDemand = _context.HeatDemandData.Sum(data => data.heatDemand);
            if(_context != null) (GasProductionCost, GasCO2Emission, OilProductionCost, OilCO2Emission) = _optimizer.OptimizeHeatProduction();
        }

    }
}
