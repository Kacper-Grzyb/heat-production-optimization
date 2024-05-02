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
            // TODO
            // Message for Peanutcho
            // The List BoilersChecked is setup in such a way already that when you select the boiler on the page
            // and click optimize the boiler names will appear in that list. What you have to do here is iterate through
            // that list and match the names to the boilers in the context and add them into the productionUnits List in this class
            // here, the rest i can set up in the boiler
            // also there will be a failsafe so that if there are not enough boilers to meet the heat demand, the koptimizer will
            // set the CanMeetHeatDemand bool to false, so you should account for that in the ui as well
        }


    }
}
