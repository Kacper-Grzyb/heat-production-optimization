using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using heat_production_optimization.Models;


namespace heat_production_optimization
{
    public class WorstScenario : IOptimizer
    {
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, double>> boilerActivations = new();
        public double TotalHeatProduction { get; set; } = 0.0;
        public double TotalElectricityProduction { get; set; } = 0.0;
        public double Expenses { get; set; } = 0.0;
        public double ConsumptionOfGas { get; set; } = 0.0;
        public double ConsumptionOfOil { get; set; } = 0.0;
        public double ConsumptionOfElectricity { get; set; } = 0.0;
        public double ProducedCO2 { get; set; } = 0.0;
        public bool CanMeetHeatDemand { get; set; } = true;

        public WorstScenario(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
        {
            HeatDemandData = heatDemandData.ToArray().OrderBy(r => r.timeFrom).ToArray();
			ProductionUnits = productionUnits.OrderByDescending(u => u.MaxHeat / u.ProductionCost).ToList(); // ordering the boilers based on the best heat to price ratio
            foreach(var record in HeatDemandData)
            {
                electricityPrices.Add(record.timeFrom, record.electricityPrice);
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
				boilerActivations.Add(timeFrame, new Dictionary<IUnit, double>());
			}
        }

        private void SortProductionUnitsCost(DateTime timeKey)
        {
            foreach(var unit in ProductionUnits)
            {
                unit.PriceToHeatRatio = ((unit.ProductionCostMWh * unit.MaxHeat) - (unit.MaxElectricity * electricityPrices[timeKey])) / unit.MaxHeat;
            }

            ProductionUnits = ProductionUnits.OrderByDescending(u => u.PriceToHeatRatio).ToList();
	    }
        private void SortProductionUnitsEmission(DateTime timeKey)
        {
            foreach (var unit in ProductionUnits)
            {
                unit.PriceToHeatRatio = ((unit.ProductionCostMWh * unit.MaxHeat) - (unit.MaxElectricity * electricityPrices[timeKey])) / unit.MaxHeat;
            }

            ProductionUnits = ProductionUnits.OrderByDescending(u => u.CO2EmissionMWh).ToList();
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
                        ConsumptionOfGas += unit.GasConsumption;
                        ConsumptionOfOil += unit.OilConsumption;
                        ProducedCO2 += unit.CO2EmissionMWh * heatProducedActual;

                        boilerActivations[currentTimeFrame].Add(unit, activationPercentage);
                    }
                    else
                    {
						boilerActivations[currentTimeFrame].Add(unit, 0.00);
					}
                }
                if(currentHeatDemand > TotalHeatProduction)
                {
                    CanMeetHeatDemand = false;
                    return;
                }
            }

            TotalHeatProduction = Math.Round(TotalHeatProduction, 2);
            TotalElectricityProduction = Math.Round(TotalElectricityProduction, 2);
            Expenses = Math.Round(Expenses, 2);
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
    public class RandomOptimizer : IOptimizer
    {
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, double>> boilerActivations = new();
        public double TotalHeatProduction { get; set; } = 0.0;
        public double TotalElectricityProduction { get; set; } = 0.0;
        public double Expenses { get; set; } = 0.0;
        public double ConsumptionOfGas { get; set; } = 0.0;
        public double ConsumptionOfOil { get; set; } = 0.0;
        public double ConsumptionOfElectricity { get; set; } = 0.0;
        public double ProducedCO2 { get; set; } = 0.0;
        public bool CanMeetHeatDemand { get; set; } = true;

        public RandomOptimizer(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
        {
            HeatDemandData = heatDemandData.ToArray().OrderBy(r => r.timeFrom).ToArray();
			ProductionUnits = productionUnits.OrderByDescending(u => u.MaxHeat / u.ProductionCost).ToList(); // ordering the boilers based on the best heat to price ratio
            Random rng = new Random(); 

            int n = ProductionUnits.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);
                (ProductionUnits[n], ProductionUnits[k]) = (ProductionUnits[k], ProductionUnits[n]);
            }
            foreach (var record in HeatDemandData)
            {
                electricityPrices.Add(record.timeFrom, record.electricityPrice);
                Tuple<DateTime, DateTime> timeFrame = new(record.timeFrom, record.timeTo);
				boilerActivations.Add(timeFrame, new Dictionary<IUnit, double>());
			}
        }

        public void OptimizeHeatProduction(OptimizationOption option)
        {
            double currentHeatDemand = 0.0;
            foreach(var record in HeatDemandData)
            {
                Tuple<DateTime, DateTime> currentTimeFrame = new(record.timeFrom, record.timeTo);
                currentHeatDemand += record.heatDemand;

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
                        ConsumptionOfGas += unit.GasConsumption;
                        ConsumptionOfOil += unit.OilConsumption;
                        ProducedCO2 += unit.CO2EmissionMWh * heatProducedActual;

                        boilerActivations[currentTimeFrame].Add(unit, activationPercentage);
                    }
                    else
                    {
						boilerActivations[currentTimeFrame].Add(unit, 0.00);
					}
                }
                if(currentHeatDemand > TotalHeatProduction)
                {
                    CanMeetHeatDemand = false;
                    return;
                }
            }

            TotalHeatProduction = Math.Round(TotalHeatProduction, 2);
            TotalElectricityProduction = Math.Round(TotalElectricityProduction, 2);
            Expenses = Math.Round(Expenses, 2);
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
}