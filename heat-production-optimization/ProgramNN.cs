using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Get user inputs for electricity price and heat demand
        Console.WriteLine("Enter the electricity price (DKK per MWh): ");
        double electricityPrice = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter the heat demand (MW): ");
        double heatDemand = Convert.ToDouble(Console.ReadLine());

        List<ProductionUnit> productionUnits = new List<ProductionUnit>
        {
            new ProductionUnit { Name = "GB", MaxHeat = 5.00, MaxElectricity = 0, ProductionCost = 500, CO2Emissions = 215, GasConsumption = 1.1, OilConsumption = 0 },
            new ProductionUnit { Name = "OB", MaxHeat = 4.00, MaxElectricity = 0, ProductionCost = 700, CO2Emissions = 265, GasConsumption = 0, OilConsumption = 1.2 },
            new ProductionUnit { Name = "GM", MaxHeat = 3.60, MaxElectricity = 2.7, ProductionCost = 1100, CO2Emissions = 640, GasConsumption = 1.9, OilConsumption = 0 },
            new ProductionUnit { Name = "EK", MaxHeat = 8.00, MaxElectricity = -8.0, ProductionCost = 50, CO2Emissions = 0, GasConsumption = 0, OilConsumption = 0 } // Electric boiler
        };

        int seed = 43;

        GeneticAlgorithm ga = new GeneticAlgorithm(productionUnits, heatDemand, electricityPrice, seed);
        Individual bestSolution = ga.Run();

        Console.WriteLine($"Total Heat Production: {bestSolution.TotalHeat} MW");
        Console.WriteLine($"Total Electricity Production: {bestSolution.TotalElectricity} MW");
        Console.WriteLine($"Expenses: {bestSolution.Expenses} DKK");
        Console.WriteLine($"Consumption of Gas: {bestSolution.GasConsumption}");
        Console.WriteLine($"Consumption of Oil: {bestSolution.OilConsumption}");
        Console.WriteLine($"Consumption of Electricity: {bestSolution.ElectricityConsumption} MWh");
        Console.WriteLine($"Produced CO2: {bestSolution.CO2} kg");

        // Output the activation percentage of each boiler
        foreach (var kvp in bestSolution.ActivationPercentages)
        {
            Console.WriteLine($"{kvp.Key.Name} Activation: {kvp.Value * 100}%");
        }
    }
}
