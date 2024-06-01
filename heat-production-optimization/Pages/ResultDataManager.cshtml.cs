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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;

namespace heat_production_optimization.Pages
{
    public class ResultDataManagerModel : PageModel
    {
        private readonly SourceDataDbContext _context;
        private IOptimizer MainOptimizer;
        private WorstScenarioOptimizer worstScenarioOptimizer;
        private RandomOptimizer randomOptimizer;
        public List<IUnit> optimizerProductionUnits { get; set; } = new List<IUnit>();
        public List<IUnit> displayProductionUnits { get; set; }
        public string errorMessage { get; set; } = string.Empty;


        public ResultDataManagerModel(SourceDataDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            displayProductionUnits = new List<IUnit>(context.productionUnits);
            if (context.productionUnitNamesForOptimization != null && context.productionUnits.Count() != 0)
            {
                foreach (OptimizerUnitNamesDataModel record in context.productionUnitNamesForOptimization)
                {
                    var unit = context.productionUnits.FirstOrDefault(u => u.Name.ToLower() == record.Name.ToLower());
                    if (unit == null) throw new Exception("The productionUnitNamesForOptimization table had a production unit name that does not exist!");
                    else optimizerProductionUnits.Add(unit);
                }
            }
            else
            {
                optimizerProductionUnits = new List<IUnit>(context.productionUnits);
            }
            worstScenarioOptimizer = new WorstScenarioOptimizer(optimizerProductionUnits, context.HeatDemandData);
            randomOptimizer = new RandomOptimizer(optimizerProductionUnits, context.HeatDemandData);
            OptimizationParameter = OptimizationOption.Cost;
            //errorMessage = _context.uiMessages.Find(MessageType.OptimizerError)?.Message ?? string.Empty;
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

		[BindProperty]
        public List<string> BoilersChecked { get; set; } = new List<string>();

        [BindProperty]
        public OptimizationOption OptimizationParameter { get; set; }

        [BindProperty]
        public OptimizerChoice OptimizerToUse { get; set; }


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

        private List<IUnit> GetUnitsForOptimizer()
        {
            List<IUnit> resultList = new();
            if (_context.productionUnitNamesForOptimization.Count() > 0)
            {
                foreach (var record in _context.productionUnitNamesForOptimization)
                {
                    var boiler = _context.productionUnits.FirstOrDefault(u => u.Name.ToLower() == record.Name.ToLower());
                    if (boiler != null)
                    {
                        resultList.Add(boiler);
                    }
                }
            }
            return resultList;
        }

        private void OptimizationProcess()
        {
            //OptimizationOption option;
            //if (OptimizationParameter.ToLower() == "both")
            //{
            //    option = OptimizationOption.Both;
            //}
            //else if (OptimizationParameter.ToLower() == "cost")
            //{
            //    option = OptimizationOption.Cost;
            //}
            //else if (OptimizationParameter.ToLower() == "emission")
            //{
            //    option = OptimizationOption.Emission;
            //}
            //else
            //{
            //    errorMessage = "At least one variable to optimize for must be selected!";
            //    _context.uiMessages.Find(MessageType.OptimizerError).Message = errorMessage;
            //    _context.SaveChanges();
            //    return;
            //}

            optimizerProductionUnits = GetUnitsForOptimizer();
            if (optimizerProductionUnits.Count() == 0) throw new Exception("No boilers to use for calculations!");

            switch(OptimizerToUse)
            {
                case OptimizerChoice.Standard:
                    MainOptimizer = new KOptimizer(optimizerProductionUnits, _context.HeatDemandData);
                    break;
                case OptimizerChoice.NeuralNetwork:
                    MainOptimizer = new NeuralNetworkOptimizer(optimizerProductionUnits, _context.HeatDemandData);
                    break;
            }

            MainOptimizer.OptimizeHeatProduction(OptimizationParameter);

            TotalHeatProduction = Math.Round(MainOptimizer.TotalHeatProduction);
            TotalElectricityProduction = Math.Round(MainOptimizer.TotalElectricityProduction);
            Expenses = Math.Round(MainOptimizer.Expenses);
            ConsumptionOfGas = Math.Round(MainOptimizer.ConsumptionOfGas);
            ConsumptionOfOil = Math.Round(MainOptimizer.ConsumptionOfOil);
            ConsumptionOfElectricity = Math.Round(MainOptimizer.ConsumptionOfElectricity);
            CO2Emission = Math.Round(MainOptimizer.ProducedCO2);

            worstScenarioOptimizer = new WorstScenarioOptimizer(optimizerProductionUnits, _context.HeatDemandData);
            worstScenarioOptimizer.OptimizeHeatProduction(OptimizationParameter);

            WorstHeat = Math.Round(worstScenarioOptimizer.TotalHeatProduction);
            WorstElectricity = Math.Round(worstScenarioOptimizer.TotalElectricityProduction);
            WorstExpenses = Math.Round(worstScenarioOptimizer.Expenses);
            WorstConsumptionOfGas = Math.Round(worstScenarioOptimizer.ConsumptionOfGas);
            WorstConsumptionOfOil = Math.Round(worstScenarioOptimizer.ConsumptionOfOil);
            WorstConsumptionOfElectricity = Math.Round(worstScenarioOptimizer.ConsumptionOfElectricity);
            WorstCO2Emission = Math.Round(worstScenarioOptimizer.ProducedCO2);

            randomOptimizer = new RandomOptimizer(optimizerProductionUnits, _context.HeatDemandData);
            randomOptimizer.OptimizeHeatProduction(OptimizationParameter);

            RandomHeat = Math.Round(randomOptimizer.TotalHeatProduction);
            RandomElectricity = Math.Round(randomOptimizer.TotalElectricityProduction);
            RandomExpenses = Math.Round(randomOptimizer.Expenses);
            RandomConsumptionOfGas = Math.Round(randomOptimizer.ConsumptionOfGas);
            RandomConsumptionOfOil = Math.Round(randomOptimizer.ConsumptionOfOil);
            RandomConsumptionOfElectricity = Math.Round(randomOptimizer.ConsumptionOfElectricity);
            RandomCO2Emission = Math.Round(randomOptimizer.ProducedCO2);

            if (!MainOptimizer.CanMeetHeatDemand)
            {
                if (_context.uiMessages.Find(MessageType.OptimizerError) == null)
                {
                    throw new Exception("The database does not have a record for an optimizer error message!");
                }
                errorMessage = "Not able to meet heat demand with chosen boiler configuration!";
                _context.uiMessages.Find(MessageType.OptimizerError).Message = errorMessage;
                _context.SaveChanges();
            }
            else
            {
                if (_context.uiMessages.Find(MessageType.OptimizerError) == null)
                {
                    throw new Exception("The database does not have a record for an optimizer error message!");
                }
                errorMessage = string.Empty;
                _context.uiMessages.Find(MessageType.OptimizerError).Message = string.Empty;
                _context.SaveChanges();
            }

            ShowResults = true;

            // Saving optimizer results to the database

            _context.optimizerResults.RemoveRange(_context.optimizerResults);
            _context.SaveChanges();
            _context.unitUsage.RemoveRange(_context.unitUsage);
            _context.SaveChanges();

            OptimizerResultsDataModel results = new OptimizerResultsDataModel()
            {
                Id = Guid.NewGuid(),
                TotalHeatProduction = MainOptimizer.TotalHeatProduction,
                TotalElectricityProduction = MainOptimizer.TotalElectricityProduction,
                Expenses = MainOptimizer.Expenses,
                ConsumptionOfGas = MainOptimizer.ConsumptionOfGas,
                ConsumptionOfOil = MainOptimizer.ConsumptionOfOil,
                ConsumptionOfElectricity = MainOptimizer.ConsumptionOfElectricity,
                ProducedCO2 = MainOptimizer.ProducedCO2
            };

            _context.optimizerResults.Add(results);
            _context.SaveChanges();

            foreach (var entry in MainOptimizer.unitUsages)
            {
                _context.unitUsage.Add(entry);
                _context.SaveChanges();
            }

            _context.SaveChanges();
            Console.WriteLine();
        }

        public void OnGet()
        {
			errorMessage = _context.uiMessages.Find(MessageType.OptimizerError)?.Message ?? string.Empty;
            double heatDemand = _context.HeatDemandData.Sum(data => data.heatDemand);

            optimizerProductionUnits = GetUnitsForOptimizer();
            if (optimizerProductionUnits.Count() == 0)
            {
                throw new Exception("No units to optimize with!");
            }

            OptimizationProcess();
        }



        public void OnPost()
        {
            Console.WriteLine();
            if (BoilersChecked != null && BoilersChecked.Any())
            {
                SelectedUnit = BoilersChecked.First();

                _context.productionUnitNamesForOptimization.RemoveRange(_context.productionUnitNamesForOptimization);
                _context.SaveChanges();

                foreach (var boilerName in BoilersChecked)
                {
                    _context.productionUnitNamesForOptimization.Add(new OptimizerUnitNamesDataModel(Guid.NewGuid(), boilerName.ToLower()));
                    _context.SaveChanges();
                }

                _context.SaveChanges();

                OptimizationProcess();
            }
            else
            {
                if (_context.uiMessages.Find(MessageType.OptimizerError) == null)
                {
                    throw new Exception("The database does not have a record for an optimizer error message!");
                }
                errorMessage = "No boilers were selected!";
                _context.uiMessages.Find(MessageType.OptimizerError).Message = errorMessage;
                _context.SaveChanges();
                _context.productionUnitNamesForOptimization.RemoveRange(_context.productionUnitNamesForOptimization);
                _context.SaveChanges();

                ShowResults = false;
            }

        }
    }

}
