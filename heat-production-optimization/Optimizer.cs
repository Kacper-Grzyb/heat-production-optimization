using System;
using Microsoft.EntityFrameworkCore;
using heat_production_optimization.Models;
using Google.OrTools.LinearSolver;
using Microsoft.AspNetCore.Mvc;

namespace heat_production_optimization
{
    
    public interface IOptimizer
    {
		public double TotalHeatProduction { get; set; }
		public double TotalElectricityProduction { get; set; }
		public double Expenses { get; set; }
		public double ConsumptionOfGas { get; set; }
		public double ConsumptionOfOil { get; set; }
		public double ConsumptionOfElectricity { get; set; }
		public double ProducedCO2 { get; set; }
        public bool CanMeetHeatDemand { get; set; }
	}

    public enum OptimizationOption
    {
        Cost,
        Emission
    }

	#region Kacper's Optimizer
	public class KOptimizer : IOptimizer
    {
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public List<UnitUsageDataModel> unitUsages = new();
        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, double>> boilerActivations = new();
        public double TotalHeatProduction { get; set; } = 0.0;
        public double TotalElectricityProduction { get; set; } = 0.0;
        public double Expenses { get; set; } = 0.0;
        public double ConsumptionOfGas { get; set; } = 0.0;
        public double ConsumptionOfOil { get; set; } = 0.0;
        public double ConsumptionOfElectricity { get; set; } = 0.0;
        public double ProducedCO2 { get; set; } = 0.0;
        public bool CanMeetHeatDemand { get; set; } = true;
        private readonly SaveToCSV saveToCSV;

        public KOptimizer(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
        {
            HeatDemandData = heatDemandData.OrderBy(r => r.timeFrom).ToArray();
			ProductionUnits = productionUnits.OrderByDescending(u => u.MaxHeat / u.ProductionCost).ToList(); // ordering the boilers based on the best heat to price ratio
            foreach(var record in HeatDemandData)
            {
                electricityPrices.Add(record.timeFrom, record.electricityPrice);
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
				boilerActivations.Add(timeFrame, new Dictionary<IUnit, double>());
			}
            saveToCSV = new SaveToCSV();
        }

        private void SortProductionUnitsCost(DateTime timeKey)
        {
            foreach(var unit in ProductionUnits)
            {
                unit.PriceToHeatRatio = ((unit.ProductionCostMWh * unit.MaxHeat) - (unit.MaxElectricity * electricityPrices[timeKey])) / unit.MaxHeat;
            }

            ProductionUnits = ProductionUnits.OrderBy(u => u.PriceToHeatRatio).ToList();
	    }

        private void SortProductionUnitsEmission(DateTime timeKey)
        {
            foreach (var unit in ProductionUnits)
            {
                unit.PriceToHeatRatio = ((unit.ProductionCostMWh * unit.MaxHeat) - (unit.MaxElectricity * electricityPrices[timeKey])) / unit.MaxHeat;
            }

            ProductionUnits = ProductionUnits.OrderBy(u => u.CO2EmissionMWh).ToList();
        }

        public void OptimizeHeatProduction(OptimizationOption option)
        {
            double currentHeatDemand = 0.0;
            foreach(var record in HeatDemandData)
            {
                Tuple<DateTime, DateTime> currentTimeFrame = new(record.timeFrom, record.timeTo);
                currentHeatDemand += record.heatDemand;

                switch(option)
                {
                    case OptimizationOption.Cost:
                        SortProductionUnitsCost(currentTimeFrame.Item1);
                        break;
                    case OptimizationOption.Emission:
                        SortProductionUnitsEmission(currentTimeFrame.Item1);
                        break;
                }

                List<UnitActivationPercentage> unitActivations = new List<UnitActivationPercentage>();
                foreach(var unit in ProductionUnits)
                {
                    if(currentHeatDemand > TotalHeatProduction)
                    {
                        double heatProducedActual;
                        if (currentHeatDemand - TotalHeatProduction > unit.MaxHeat) heatProducedActual = unit.MaxHeat;
                        else heatProducedActual = currentHeatDemand - TotalHeatProduction;
						double activationPercentage = Math.Round(heatProducedActual / unit.MaxHeat, 2);

						TotalHeatProduction += heatProducedActual;
                        if(unit.MaxElectricity != 0)
                        {
                            if (unit.MaxElectricity > 0) TotalElectricityProduction += unit.MaxElectricity * activationPercentage;
                            else ConsumptionOfElectricity -= unit.MaxElectricity * activationPercentage;
                            Expenses += unit.MaxElectricity * activationPercentage * record.electricityPrice;
                        }

                        Expenses -= unit.ProductionCostMWh * heatProducedActual;
                        ConsumptionOfGas += unit.GasConsumption * heatProducedActual;
                        ConsumptionOfOil += unit.OilConsumption * heatProducedActual;
                        ProducedCO2 += unit.CO2EmissionMWh * heatProducedActual;

                        boilerActivations[currentTimeFrame].Add(unit, activationPercentage);
                        unitActivations.Add(new UnitActivationPercentage() { ActivationPercentage = activationPercentage, Unit = (ProductionUnitDataModel)unit });
                    }
                    else
                    {
						boilerActivations[currentTimeFrame].Add(unit, 0.00);
						unitActivations.Add(new UnitActivationPercentage() { Id = Guid.NewGuid(), ActivationPercentage = 0.0, Unit = (ProductionUnitDataModel)unit });
					}
                }

                unitUsages.Add(new UnitUsageDataModel() { Id = Guid.NewGuid(), DateInterval = new DateInterval() { Id = Guid.NewGuid(), TimeFrom = record.timeFrom, TimeTo = record.timeTo }, ActivationPercentages = unitActivations });

                if(currentHeatDemand > TotalHeatProduction)
                {
                    CanMeetHeatDemand = false;
                    return;
                }
            }

            TotalHeatProduction = Math.Round(TotalHeatProduction, 2);
            TotalElectricityProduction = Math.Round(TotalElectricityProduction, 2);
            Expenses = Math.Abs(Math.Round(Expenses, 2));
            ConsumptionOfGas = Math.Round(ConsumptionOfGas, 2);
            ConsumptionOfOil = Math.Round(ConsumptionOfOil, 2);
            ConsumptionOfElectricity = Math.Round(ConsumptionOfElectricity, 2);
            ProducedCO2 = Math.Round(ProducedCO2, 2);


            foreach(var record in HeatDemandData)
            {
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
                Console.Write(timeFrame);

                HourlyOptimization newHour = new HourlyOptimization
                {
                    timeFrom = record.timeFrom,
                    timeTo = record.timeTo,
                };
                foreach (var unit in ProductionUnits)
                {
                    Console.Write($"{unit.Alias} {boilerActivations[timeFrame][unit]} ");
                    newHour[unit] = boilerActivations[timeFrame][unit];

                }
                HourlyOptimization.HourlyOptimizations.Add(newHour);


                Console.WriteLine();

            }
            //SaveToCSV.SaveOptimization();

	    }

    }


    #endregion










    #region Sebi's Optimizer

    public class SOptimizer : IOptimizer
    {
        //private SourceDataDbContext _context;
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, bool>> boilerActivations = new();
        public double TotalHeatProduction { get; set; } = 0.0;
        public double TotalElectricityProduction { get; set; } = 0.0;
        public double Expenses { get; set; } = 0.0;
        public double ConsumptionOfGas { get; set; } = 0.0;
        public double ConsumptionOfOil { get; set; } = 0.0;
        public double ConsumptionOfElectricity { get; set; } = 0.0;
        public double ProducedCO2 { get; set; } = 0.0;
        public bool CanMeetHeatDemand { get; set; } = true;

        /* Needed data: 
         *  Max heat production from result configuration 
         *  Max electricity production
         *  Max electricity consumption
         *  Expenses and profit
         *  Consumption of primary energy
         *  Produced C02
         *  
         * Sort either by highest profit or lowest co2 emissions
         */

        public SOptimizer(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
        {
            HeatDemandData = heatDemandData.OrderBy(r => r.timeFrom).ToArray();
			ProductionUnits = productionUnits.OrderByDescending(u => u.MaxHeat / u.ProductionCost).ToList(); // ordering the boilers based on the best heat to price ratio
            foreach(var record in HeatDemandData)
            {
                electricityPrices.Add(record.timeFrom, record.electricityPrice);
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
				boilerActivations.Add(timeFrame, new Dictionary<IUnit, bool>());
			}
        }
        public (double TotalCost, double TotalCO2Emissions, double TotalElectricityGenerated, double TotalHeatProduced) OptimizeCost(double heatDemand, double electricityPrice)
        {
            //DON'T USE
            //Only use if machines can only produce whole MWh
            //heatDemand = Math.Ceiling(heatDemand);
            //DON'T USE

            Solver solver = Solver.CreateSolver("SCIP");

            

            IUnit[] units = ProductionUnits.ToArray();

            Variable[] unitOperation = new Variable[units.Length];
            for (int i = 0; i < units.Length; i++)
            {
                unitOperation[i] = solver.MakeNumVar(0, 1, $"operation_{units[i].Name}");
            }

            Objective objective = solver.Objective();
            for (int i = 0; i < units.Length; i++)
            {
                double unitCost = units[i].ProductionCost;
                objective.SetCoefficient(unitOperation[i], unitCost);

                if (units[i] is GasMotor || units[i] is ElectricBoiler)
                {
                    // Directly setting negative revenue (as a reduction in costs) in the objective
                    double maxElectricity = (units[i] is GasMotor) ? ((GasMotor)units[i]).MaxElectricity : ((ElectricBoiler)units[i]).MaxElectricity;
                    double revenue = maxElectricity * electricityPrice - unitCost;
                    objective.SetCoefficient(unitOperation[i], -revenue);
                }
            }

            objective.SetMinimization();

            Constraint demandConstraint = solver.MakeConstraint(heatDemand, double.PositiveInfinity, "heatDemand");
            for (int i = 0; i < units.Length; i++)
            {
                demandConstraint.SetCoefficient(unitOperation[i], units[i].MaxHeat);
            }

            Solver.ResultStatus resultStatus = solver.Solve();

            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                System.Console.WriteLine("Solution for the hour:");
                double totalCO2Emissions = 0;
                double totalElectricityGenerated = 0;
                double totalHeatProduced = 0;
                for (int i = 0; i < units.Length; i++)
                {
                    System.Console.WriteLine($"{units[i].Name} operates at {Math.Round(unitOperation[i].SolutionValue() * 100)}% capacity;");
                    totalCO2Emissions += unitOperation[i].SolutionValue() * units[i].CO2Emission;
                    if (units[i] is GasMotor)
                    {
                        totalElectricityGenerated += unitOperation[i].SolutionValue() * ((GasMotor)units[i]).MaxElectricity;
                    }
                    else if (units[i] is ElectricBoiler)
                    {
                        totalElectricityGenerated += unitOperation[i].SolutionValue() * ((ElectricBoiler)units[i]).MaxElectricity;
                    }
                    totalHeatProduced += unitOperation[i].SolutionValue() * units[i].MaxHeat;
                }
                System.Console.WriteLine($"Total Cost: {Math.Round(objective.Value())}");
                Expenses += objective.Value();
                System.Console.WriteLine($"Total CO2 Emissions: {Math.Round(totalCO2Emissions)}");
                ProducedCO2 += totalCO2Emissions;
                System.Console.WriteLine($"Total Electricity Generated: {Math.Round(totalElectricityGenerated)}");
                TotalElectricityProduction += totalElectricityGenerated;
                System.Console.WriteLine($"Total Heat Produced: {Math.Round(totalHeatProduced)}");
                TotalHeatProduction += totalHeatProduced;

                return (objective.Value(), totalCO2Emissions, totalElectricityGenerated, totalHeatProduced);
            }
            else
            {
                System.Console.WriteLine("The problem does not have an optimal solution.");
                return (double.NaN, 0, 0, 0);
            }
        }

        public void OptimizingCycle(SourceDataDbContext context)
        {
            var heatDemandArray = context.HeatDemandData.ToArray();
            for(int i = 0; i < heatDemandArray.Length; i++)
            {
                double HeatDemandForHour = heatDemandArray[i].heatDemand;
                double ElectricyPriceForHour = heatDemandArray[i].electricityPrice;
                OptimizeCost(HeatDemandForHour, ElectricyPriceForHour);
            }
        }
        

    }

    #endregion

}
