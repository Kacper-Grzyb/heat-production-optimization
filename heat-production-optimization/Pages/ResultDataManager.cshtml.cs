using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;

namespace heat_production_optimization.Pages
{
    public class ResultDataManagerModel : PageModel
    {
        private readonly SourceDataDbContext _context;
        private readonly KOptimizer kOptimizer;
        private readonly SOptimizer sOptimizer;
        public List<IUnit> productionUnits { get; set; }

        public ResultDataManagerModel(SourceDataDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            productionUnits = _context.productionUnits;
            kOptimizer = new KOptimizer(context.productionUnits, context.HeatDemandData);
            sOptimizer = new SOptimizer(context.productionUnits, context.HeatDemandData);
        }

		public double TotalHeatProduction { get; set; }
		public double TotalElectricityProduction { get; set; }
		public double Expenses { get; set; }
		public double ConsumptionOfGas { get; set; }
		public double ConsumptionOfOil { get; set; }
		public double ConsumptionOfElectricity { get; set; }
		public double CO2Emission { get; set; }

        [BindProperty]
        public List<string> BoilersChecked { get; set; }

        //public void OnGet()
        //{
        //    double heatDemand = _context.HeatDemandData.Sum(data => data.heatDemand);
        //    sOptimizer.OptimizingCycle(_context);

        //    TotalHeatProduction = Math.Round(sOptimizer.TotalHeatProduction);
        //    TotalElectricityProduction = Math.Round(sOptimizer.TotalElectricityProduction);
        //    Turnover = Math.Round(sOptimizer.Turnover);
        //    ConsumptionOfGas = Math.Round(sOptimizer.ConsumptionOfGas);
        //    ConsumptionOfOil = Math.Round(sOptimizer.ConsumptionOfOil);
        //    ConsumptionOfElectricity = Math.Round(sOptimizer.ConsumptionOfElectricity);
        //    CO2Emission = Math.Round(sOptimizer.ProducedCO2);

        //}

        public void OnGet()
        {
            double heatDemand = _context.HeatDemandData.Sum(data => data.heatDemand);
            kOptimizer.OptimizeHeatProduction(OptimizationOption.Emission);

            TotalHeatProduction = Math.Round(kOptimizer.TotalHeatProduction);
            TotalElectricityProduction = Math.Round(kOptimizer.TotalElectricityProduction);
            Expenses = Math.Round(kOptimizer.Expenses);
            ConsumptionOfGas = Math.Round(kOptimizer.ConsumptionOfGas);
            ConsumptionOfOil = Math.Round(kOptimizer.ConsumptionOfOil);
            ConsumptionOfElectricity = Math.Round(kOptimizer.ConsumptionOfElectricity);
            CO2Emission = Math.Round(kOptimizer.ProducedCO2);
        }

        public void OnPost()
        {
            Console.WriteLine();
            //TODO
            //iterate through the boiler names in boilerchecklist and add them to the optimizerboilers
            //add a button to optimize and implement a failsafe if there are not enough boilers
        }


    }
}
