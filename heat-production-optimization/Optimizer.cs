namespace heat_production_optimization
{
    public class Optimizer
    {
        private GasBoiler gasBoiler;
        private OilBoiler oilBoiler;

        public Optimizer()
        {
            gasBoiler = new GasBoiler("GB", maxHeat: 5, productionCost: 500, cO2Emission: 215, gasConsumption: 1.1);
            oilBoiler = new OilBoiler("OB", maxHeat: 4, productionCost: 700, cO2Emission: 265, oilConsumption: 1.2);
        }

        public void OptimizeHeatProduction(double heatDemand)
        {
            double remainingHeatDemand = heatDemand;

            // Check if GasBoiler alone can meet the demand
            if (gasBoiler.MaxHeat >= remainingHeatDemand)
            {
                Console.WriteLine($"Using {gasBoiler.Name} to meet the heat demand.");
                // Calculate production cost and CO2 emission for GasBoiler
                double gasProductionCost = gasBoiler.ProductionCost * remainingHeatDemand;
                double gasCO2Emission = gasBoiler.CO2Emission * remainingHeatDemand;
                Console.WriteLine($"Production cost: {gasProductionCost}, CO2 emission: {gasCO2Emission}");
                return;
            }

            // Use GasBoiler as much as possible
            double gasHeatProduced = gasBoiler.MaxHeat;
            remainingHeatDemand -= gasHeatProduced;

            // Check if OilBoiler is needed to meet the remaining demand
            if (remainingHeatDemand > 0 && oilBoiler.MaxHeat >= remainingHeatDemand)
            {
                Console.WriteLine($"Using {gasBoiler.Name} and {oilBoiler.Name} to meet the heat demand.");
                // Calculate production cost and CO2 emission for both boilers
                double gasProductionCost = gasBoiler.ProductionCost * gasHeatProduced;
                double gasCO2Emission = gasBoiler.CO2Emission * gasHeatProduced;
                double oilProductionCost = oilBoiler.ProductionCost * remainingHeatDemand;
                double oilCO2Emission = oilBoiler.CO2Emission * remainingHeatDemand;
                double totalProductionCost = gasProductionCost + oilProductionCost;
                double totalCO2Emission = gasCO2Emission + oilCO2Emission;
                Console.WriteLine($"Total production cost: {totalProductionCost}, Total CO2 emission: {totalCO2Emission}");
                return;
            }

            // GasBoiler and OilBoiler combined cannot meet the demand
            Console.WriteLine("Insufficient heat production capacity.");
        }
    }

}
