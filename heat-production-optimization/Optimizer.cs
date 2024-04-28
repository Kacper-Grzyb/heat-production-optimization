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
        public double Turnover = 0.0;
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

}
