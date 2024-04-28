using System;
using Microsoft.EntityFrameworkCore;
using heat_production_optimization.Models;

namespace heat_production_optimization
{
    public class Optimizer
    {
        private IUnit? _gasBoiler;
        private IUnit? _oilBoiler;
        
        public (double, double, double, double) OptimizeHeatProduction(double heatDemand, SourceDataDbContext context)
        {
            return (1, 1, 1, 1);
        }
    }

}
