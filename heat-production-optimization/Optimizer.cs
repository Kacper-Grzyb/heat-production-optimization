using System;
using Microsoft.EntityFrameworkCore;
using heat_production_optimization.Models;

namespace heat_production_optimization
{
    public class Optimizer
    {
        //private SourceDataDbContext _context;
        private List<IUnit> ProductionUnits;
        private DbSet<HeatDemandDataModel> HeatDemandData;
        
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
            HeatDemandData = heatDemandData;
            ProductionUnits = productionUnits;
            productionUnits = productionUnits.OrderBy(u => u.MaxHeat/u.ProductionCost).ToList(); // ordering the boilers based on the best heat to price ratio
        }

        public (double, double, double, double) OptimizeHeatProduction()
        {
            return (1, 1, 1, 1);
        }
    }

}
