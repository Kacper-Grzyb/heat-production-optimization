using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace heat_production_optimization.Pages
{
    public class ResultDataManagerModel : PageModel
    {
        private readonly SourceDataDbContext _context;
        private KOptimizer kOptimizer;
        private readonly SOptimizer sOptimizer;
        private readonly WorstScenario worstScenario;
        private readonly RandomOptimizer randomOptimizer;
        public List<IUnit> optimizerProductionUnits { get; set; } = new List<IUnit>();
        public List<IUnit> displayProductionUnits { get; set; }


        public ResultDataManagerModel(SourceDataDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            displayProductionUnits = new List<IUnit>(context.productionUnits);
            if (context.productionUnitNamesForOptimization != null && context.productionUnitNamesForOptimization.Count() != 0)
            {
                foreach (OptimizerUnitNamesDataModel record in context.productionUnitNamesForOptimization)
                {
                    var unit = context.productionUnits.FirstOrDefault(u => u.Name == record.Name);
                    if (unit == null) throw new Exception("The productionUnitNamesForOptimization table had a production unit name that does not exist!");
                    else optimizerProductionUnits.Add(unit);

                }
            }
            else
            {
                optimizerProductionUnits = new List<IUnit>(context.productionUnits);
            }
            kOptimizer = new KOptimizer(optimizerProductionUnits, context.HeatDemandData);
            sOptimizer = new SOptimizer(optimizerProductionUnits, context.HeatDemandData);
            worstScenario = new WorstScenario(optimizerProductionUnits, context.HeatDemandData);
            randomOptimizer = new RandomOptimizer(optimizerProductionUnits, context.HeatDemandData);
        }

        public double TotalHeatProduction { get; set; }
        public double TotalElectricityProduction { get; set; }
        public double Expenses { get; set; }
        public double ConsumptionOfGas { get; set; }
        public double ConsumptionOfOil { get; set; }
        public double ConsumptionOfElectricity { get; set; }
        public double CO2Emission { get; set; }

		public bool ShowResults { get; set; } = false;
		public string SelectedUnit { get; set; }
		public bool IsInitialLoad { get; private set; }

		[BindProperty]
        public List<string> BoilersChecked { get; set; } = new List<string>();


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
            IsInitialLoad = true;

            

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
            IsInitialLoad = false;


            if (BoilersChecked != null && BoilersChecked.Any())
            {
                SelectedUnit = BoilersChecked.First();

                optimizerProductionUnits = new List<IUnit>();
                _context.productionUnitNamesForOptimization.RemoveRange(_context.productionUnitNamesForOptimization);
                _context.SaveChanges();

                foreach (var boilerName in BoilersChecked)
                {
                    var boiler = _context.productionUnits.FirstOrDefault(u => u.Name == boilerName);
                    if (boiler != null)
                    {
                        optimizerProductionUnits.Add(boiler);
                        _context.productionUnitNamesForOptimization.Add(new OptimizerUnitNamesDataModel(Guid.NewGuid(), boilerName));
                    }
                }

                _context.SaveChanges();
                kOptimizer = new KOptimizer(optimizerProductionUnits, _context.HeatDemandData);
                kOptimizer.OptimizeHeatProduction(OptimizationOption.Cost);

                TotalHeatProduction = Math.Round(kOptimizer.TotalHeatProduction);
                TotalElectricityProduction = Math.Round(kOptimizer.TotalElectricityProduction);
                Expenses = Math.Round(kOptimizer.Expenses);
                ConsumptionOfGas = Math.Round(kOptimizer.ConsumptionOfGas);
                ConsumptionOfOil = Math.Round(kOptimizer.ConsumptionOfOil);
                ConsumptionOfElectricity = Math.Round(kOptimizer.ConsumptionOfElectricity);
                CO2Emission = Math.Round(kOptimizer.ProducedCO2);

                if (!kOptimizer.CanMeetHeatDemand)
                    Console.WriteLine("Not able to meet demand!");

                ShowResults = true;
            }
            else
            {
                Console.WriteLine("No Boiler selected!");
                ShowResults = false;
            }
        }


    }
}
