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
        private readonly WorstScenario worstScenario;
        private readonly RandomOptimizer randomOptimizer;
        public List<IUnit> productionUnits { get; set; }

        public ResultDataManagerModel(SourceDataDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            productionUnits = _context.productionUnits;
            kOptimizer = new KOptimizer(context.productionUnits, context.HeatDemandData);
            sOptimizer = new SOptimizer(context.productionUnits, context.HeatDemandData);
            worstScenario = new WorstScenario(context.productionUnits, context.HeatDemandData);
            randomOptimizer = new RandomOptimizer(context.productionUnits, context.HeatDemandData);
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


        //Worst case properties
        public double WorstHeat { get; set; }
        public double WorstElectricity { get; set; }
        public double WorstExpenses { get; set; }
        public double WorstConsumptionOfGas { get; set; }
        public double WorstConsumptionOfOil { get; set; }
        public double WorstConsumptionOfElectricity { get; set; }
        public double WorstCO2Emission { get; set; }

        //Average case properties
        public double RandomHeat { get; set; }
        public double RandomElectricity { get; set; }
        public double RandomExpenses { get; set; }
        public double RandomConsumptionOfGas { get; set; }
        public double RandomConsumptionOfOil { get; set; }
        public double RandomConsumptionOfElectricity { get; set; }
        public double RandomCO2Emission { get; set; }


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
            kOptimizer.OptimizeHeatProduction(OptimizationOption.Cost);

            TotalHeatProduction = Math.Round(kOptimizer.TotalHeatProduction);
            TotalElectricityProduction = Math.Round(kOptimizer.TotalElectricityProduction);
            Expenses = Math.Round(kOptimizer.Expenses);
            ConsumptionOfGas = Math.Round(kOptimizer.ConsumptionOfGas);
            ConsumptionOfOil = Math.Round(kOptimizer.ConsumptionOfOil);
            ConsumptionOfElectricity = Math.Round(kOptimizer.ConsumptionOfElectricity);
            CO2Emission = Math.Round(kOptimizer.ProducedCO2);


            worstScenario.OptimizeHeatProduction(OptimizationOption.Cost);

            WorstHeat = Math.Round(worstScenario.TotalHeatProduction);
            WorstElectricity = Math.Round(worstScenario.TotalElectricityProduction);
            WorstExpenses = Math.Round(worstScenario.Expenses);
            WorstConsumptionOfGas = Math.Round(worstScenario.ConsumptionOfGas);
            WorstConsumptionOfOil = Math.Round(worstScenario.ConsumptionOfOil);
            WorstConsumptionOfElectricity = Math.Round(worstScenario.ConsumptionOfElectricity);
            WorstCO2Emission = Math.Round(worstScenario.ProducedCO2);

            randomOptimizer.OptimizeHeatProduction(OptimizationOption.Cost);

            RandomHeat = Math.Round(randomOptimizer.TotalHeatProduction);
            RandomElectricity = Math.Round(randomOptimizer.TotalElectricityProduction);
            RandomExpenses = Math.Round(randomOptimizer.Expenses);
            RandomConsumptionOfGas = Math.Round(randomOptimizer.ConsumptionOfGas);
            RandomConsumptionOfOil = Math.Round(randomOptimizer.ConsumptionOfOil);
            RandomConsumptionOfElectricity = Math.Round(randomOptimizer.ConsumptionOfElectricity);
            RandomCO2Emission = Math.Round(randomOptimizer.ProducedCO2);
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

            if (BoilersChecked != null && BoilersChecked.Any())
            {
                // Clear the existing productionUnits list
                productionUnits.Clear();

                // Iterate through the selected boiler names
                foreach (var boilerName in BoilersChecked)
                {
                    // Find the boiler in the context based on the name
                    var boiler = _context.productionUnits.FirstOrDefault(u => u.Name == boilerName);

                    // Add the boiler to the productionUnits list if found
                    if (boiler != null)
                    {
                        productionUnits.Add(boiler);
                    }
                }

                // Perform the optimization with the updated productionUnits list
                kOptimizer.OptimizeHeatProduction(OptimizationOption.Cost);

                // Update the UI with the optimized results
                TotalHeatProduction = Math.Round(kOptimizer.TotalHeatProduction);
                TotalElectricityProduction = Math.Round(kOptimizer.TotalElectricityProduction);
                Expenses = Math.Round(kOptimizer.Expenses);
                ConsumptionOfGas = Math.Round(kOptimizer.ConsumptionOfGas);
                ConsumptionOfOil = Math.Round(kOptimizer.ConsumptionOfOil);
                ConsumptionOfElectricity = Math.Round(kOptimizer.ConsumptionOfElectricity);
                CO2Emission = Math.Round(kOptimizer.ProducedCO2);

                // Check if there are enough boilers to meet the heat demand
                if (!kOptimizer.CanMeetHeatDemand)
                {
                    // If not, set a message or perform any necessary action
                    // For example, you can set a flag to display a warning in the UI
                    Console.WriteLine("Not able to meet demand!");
                }
            }
            else
            {
                // Handle case where no boilers are selected
                // You can set a message or perform any necessary action
                Console.WriteLine("No Boilder selected!");
            }
        }


    }
}
