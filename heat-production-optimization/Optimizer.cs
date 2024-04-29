using System;
using Microsoft.EntityFrameworkCore;
using heat_production_optimization.Models;
using Google.OrTools.LinearSolver;

namespace heat_production_optimization
{
    public interface IOptimizer
    {
		public double TotalHeatProduction { get; set; }
		public double TotalElectricityProduction { get; set; }
		public double Turnover { get; set; }
		public double ConsumptionOfGas { get; set; }
		public double ConsumptionOfOil { get; set; }
		public double ConsumptionOfElectricity { get; set; }
		public double ProducedCO2 { get; set; }
	}

    public class KOptimizer : IOptimizer
    {
        //private SourceDataDbContext _context;
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, bool>> boilerActivations = new();
        public double TotalHeatProduction { get; set; } = 0.0;
        public double TotalElectricityProduction { get; set; } = 0.0;
        public double Turnover { get; set; } = 0.0;
        public double ConsumptionOfGas { get; set; } = 0.0;
        public double ConsumptionOfOil { get; set; } = 0.0;
        public double ConsumptionOfElectricity { get; set; } = 0.0;
        public double ProducedCO2 { get; set; } = 0.0;
        
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

        public KOptimizer(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
        {
            HeatDemandData = heatDemandData.ToArray().OrderBy(r => r.timeFrom).ToArray();
			ProductionUnits = productionUnits.OrderByDescending(u => u.MaxHeat / u.ProductionCost).ToList(); // ordering the boilers based on the best heat to price ratio
            foreach(var record in HeatDemandData)
            {
                electricityPrices.Add(record.timeFrom, record.electricityPrice);
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
				boilerActivations.Add(timeFrame, new Dictionary<IUnit, bool>());
			}
        }

        private void SortProductionUnits(DateTime timeKey)
        {
            foreach(var unit in ProductionUnits)
            {
                unit.PriceToHeatRatio = unit.ProductionCost - (unit.MaxElectricity * electricityPrices[timeKey]) / unit.MaxHeat;
            }

            ProductionUnits = ProductionUnits.OrderBy(u => u.PriceToHeatRatio).ToList();
	    }

        public void OptimizeHeatProduction()
        {
            double currentHeatDemand = 0.0;
            foreach(var record in HeatDemandData)
            {
                Tuple<DateTime, DateTime> currentTimeFrame = new(record.timeFrom, record.timeTo);
                currentHeatDemand += record.heatDemand;

                SortProductionUnits(currentTimeFrame.Item1);

                foreach(var unit in ProductionUnits)
                {
                    if(currentHeatDemand > TotalHeatProduction)
                    {
                        TotalHeatProduction += unit.MaxHeat;
                        if(unit.MaxElectricity != 0)
                        {
                            TotalElectricityProduction += unit.MaxElectricity;
                            Turnover += unit.MaxElectricity * record.electricityPrice;
                        }

                        ConsumptionOfGas += unit.GasConsumption;
                        ConsumptionOfOil += unit.OilConsumption;
                        ConsumptionOfElectricity += unit.MaxElectricity < 0 ? Math.Abs(unit.MaxElectricity) : 0;
                        ProducedCO2 += unit.CO2Emission;

                        boilerActivations[currentTimeFrame].Add(unit, true);
                    }
                    else
                    {
						boilerActivations[currentTimeFrame].Add(unit, false);
					}
                }
            }

            TotalHeatProduction = Math.Round(TotalHeatProduction, 2);
            TotalElectricityProduction = Math.Round(TotalElectricityProduction, 2);
            Turnover = Math.Round(Turnover, 2);
            ConsumptionOfGas = Math.Round(ConsumptionOfGas, 2);
            ConsumptionOfOil = Math.Round(ConsumptionOfOil, 2);
            ConsumptionOfElectricity = Math.Round(ConsumptionOfElectricity, 2);
            ProducedCO2 = Math.Round(ProducedCO2, 2);

            foreach(var record in HeatDemandData)
            {
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
                Console.Write(timeFrame);
                foreach(var unit in ProductionUnits)
                {
                    Console.Write($"{unit.Alias} {boilerActivations[timeFrame][unit]} ");
                }
                Console.WriteLine();
            }
	    }
    }



















    public class SOptimizer : IOptimizer
    {
        //private SourceDataDbContext _context;
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, bool>> boilerActivations = new();
        public double TotalHeatProduction { get; set; } = 0.0;
        public double TotalElectricityProduction { get; set; } = 0.0;
        public double Turnover { get; set; } = 0.0;
        public double ConsumptionOfGas { get; set; } = 0.0;
        public double ConsumptionOfOil { get; set; } = 0.0;
        public double ConsumptionOfElectricity { get; set; } = 0.0;
        public double ProducedCO2 { get; set; } = 0.0;
        
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
            HeatDemandData = heatDemandData.ToArray().OrderBy(r => r.timeFrom).ToArray();
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
                Turnover += objective.Value();
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

}
