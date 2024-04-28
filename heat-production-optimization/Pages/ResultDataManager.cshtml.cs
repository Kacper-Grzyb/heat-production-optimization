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

		public double TotalHeatProduction { get; set; }
		public double TotalElectricityProduction { get; set; }
		public double Expenses { get; set; }
		public double Profit { get; set; }
		public double ConsumptionOfGas { get; set; }
		public double ConsumptionOfOil { get; set; }
		public double ConsumptionOfElectricity { get; set; }
		public double CO2Emission { get; set; }

		public void OnGet()
        {
			double heatDemand = _context.HeatDemandData.Sum(data => data.heatDemand);
            _optimizer.OptimizeHeatProduction();

            TotalHeatProduction = _optimizer.TotalHeatProduction;
            TotalElectricityProduction = _optimizer.TotalElectricityProduction;
            Expenses = _optimizer.Expenses;
            Profit = _optimizer.Profit;
            ConsumptionOfGas = _optimizer.ConsumptionOfGas;
            ConsumptionOfOil = _optimizer.ConsumptionOfOil;
            ConsumptionOfElectricity = _optimizer.ConsumptionOfElectricity;
            CO2Emission = _optimizer.ProducedCO2;

        }

    }
}
