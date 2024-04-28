using System;
using Microsoft.EntityFrameworkCore;
using heat_production_optimization.Models;

namespace heat_production_optimization
{
    public class Optimizer
    {
        //private SourceDataDbContext _context;
        private List<IUnit> ProductionUnits;
        private HeatDemandDataModel[] HeatDemandData;
        private Dictionary<DateTime, double> electricityPrices = new();

        public Dictionary<Tuple<DateTime, DateTime>, Dictionary<IUnit, bool>> boilerActivations = new();
        public double TotalHeatProduction = 0.0;
        public double TotalElectricityProduction = 0.0;
        public double Expenses = 0.0;
        public double Profit = 0.0;
        public double ConsumptionOfGas = 0.0;
        public double ConsumptionOfOil = 0.0;
        public double ConsumptionOfElectricity = 0.0;
        public double ProducedCO2 = 0.0;
        
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

        public Optimizer(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
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
            List<Tuple<IUnit, double>> unitValues = new();

            foreach(var unit in ProductionUnits)
            {
                double currentProductionCost = unit.ProductionCost ?? 1;
                double currentHeatProduction = unit.MaxHeat ?? 1;
                if (unit.MaxElectricity != null)
                {
					currentProductionCost -= unit.MaxElectricity * electricityPrices[timeKey] ?? 0;
                }
                double ratio = currentProductionCost / currentHeatProduction;
                unitValues.Add(new Tuple<IUnit, double> (unit, ratio));
            }

            List<IUnit> newProductionUnits = new();
            foreach (var item in unitValues.OrderBy(i => i.Item2)) newProductionUnits.Add(item.Item1);
            ProductionUnits = newProductionUnits;
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
                        TotalHeatProduction += unit.MaxHeat ?? 0;
                        if(unit.MaxElectricity != null)
                        {
                            if(unit.MaxElectricity > 0)
                            {
                                TotalElectricityProduction += unit.MaxElectricity ?? 0;
                                Profit += unit.MaxElectricity * record.electricityPrice ?? 0;
                            }
                            else
                            {
                                Expenses -= unit.MaxElectricity * record.electricityPrice ?? 0;
                            }
                        }

                        ConsumptionOfGas += unit.GasConsumption ?? 0;
                        ConsumptionOfOil += unit.OilConsumption ?? 0;
                        ConsumptionOfElectricity += unit.MaxElectricity < 0 ? unit.MaxElectricity ?? 0 : 0;
                        ProducedCO2 += unit.CO2Emission ?? 0;

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
            Expenses = Math.Round(Expenses, 2);
            Profit = Math.Round(Profit, 2);
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
