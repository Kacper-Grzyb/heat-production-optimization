using System;

// namespace heat_production_optimization
// {
//     public class Optimizer
//     {
//         private GasBoiler gasBoiler;
//         private OilBoiler oilBoiler;

//         public Optimizer()
//         {
//             // Initialize GasBoiler and OilBoiler with their respective capacities and parameters
//             gasBoiler = new GasBoiler("GB", maxHeat: 5, productionCost: 500, cO2Emission: 215, gasConsumption: 1.1);
//             oilBoiler = new OilBoiler("OB", maxHeat: 4, productionCost: 700, cO2Emission: 265, oilConsumption: 1.2);
//         }

//         public OptimizeHeatProduction()
//         {
//             double remainingHeatDemand = heatDemand;

//             // Check if GasBoiler alone can meet the demand
//             if (gasBoiler.MaxHeat >= remainingHeatDemand)
//             {
//                 // GasBoiler can meet the demand
//                 Console.WriteLine($"Using {gasBoiler.Name} to meet the heat demand.");
//                 double gasProductionCost = gasBoiler.ProductionCost * remainingHeatDemand;
//                 double gasCO2Emission = gasBoiler.CO2Emission * remainingHeatDemand;
//                 return;
//             }

//             // Use GasBoiler as much as possible
//             double gasHeatProduced = gasBoiler.MaxHeat;
//             remainingHeatDemand -= gasHeatProduced;

//             // Check if OilBoiler is needed to meet the remaining demand
//             if (remainingHeatDemand > 0 && oilBoiler.MaxHeat >= remainingHeatDemand)
//             {
//                 // Both GasBoiler and OilBoiler are needed
//                 Console.WriteLine($"Using {gasBoiler.Name} and {oilBoiler.Name} to meet the heat demand.");
//                 double gasProductionCost = gasBoiler.ProductionCost * gasHeatProduced;
//                 double gasCO2Emission = gasBoiler.CO2Emission * gasHeatProduced;
//                 double oilProductionCost = oilBoiler.ProductionCost * remainingHeatDemand;
//                 double oilCO2Emission = oilBoiler.CO2Emission * remainingHeatDemand;
//                 double totalProductionCost = gasProductionCost + oilProductionCost;
//                 double totalCO2Emission = gasCO2Emission + oilCO2Emission;
//                 return;
//             }

//             // GasBoiler and OilBoiler combined cannot meet the demand
//             Console.WriteLine("Insufficient heat production capacity.");
//             
//         }
//     }
// }

using System;

namespace heat_production_optimization
{
    public class Optimizer
    {
        private readonly GasBoiler _gasBoiler;
        private readonly OilBoiler _oilBoiler;

        public Optimizer()
        {
            _gasBoiler = new GasBoiler("GB", 5, 500, 215, 1.1);
            _oilBoiler = new OilBoiler("OB", 4, 700, 265, 1.2);
        }

        public (double, double, double, double) OptimizeHeatProduction(double heatDemand)
        {
            double gasProductionCost = 0;
            double gasCO2Emission = 0;
            double oilProductionCost = 0;
            double oilCO2Emission = 0;

            if (heatDemand <= _gasBoiler.MaxHeat)
            {
                gasProductionCost = heatDemand * _gasBoiler.ProductionCost;
                gasCO2Emission = heatDemand * _gasBoiler.CO2Emission;
            }
            else
            {
                gasProductionCost = _gasBoiler.MaxHeat * _gasBoiler.ProductionCost;
                gasCO2Emission = _gasBoiler.MaxHeat * _gasBoiler.CO2Emission;

                double remainingHeatDemand = heatDemand - _gasBoiler.MaxHeat;
                double oilHeatProduced = Math.Min(remainingHeatDemand, _oilBoiler.MaxHeat);
                oilProductionCost = oilHeatProduced * _oilBoiler.ProductionCost;
                oilCO2Emission = oilHeatProduced * _oilBoiler.CO2Emission;

                gasProductionCost += oilProductionCost;
                gasCO2Emission += oilCO2Emission;
            }

            return (gasProductionCost, gasCO2Emission, oilProductionCost, oilCO2Emission);
        }
    }

}
