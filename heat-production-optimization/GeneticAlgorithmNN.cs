using System;
using System.Collections.Generic;
using System.Linq;

public class GeneticAlgorithm
{
    private static Random random = new Random();
    private const double MutationRate = 0.01;
    private const int GeneLength = 4;
    private const int PopulationSize = 200;
    private const int MaxGenerations = 1000;
    private const int StagnationLimit = 100;

    public List<ProductionUnit> ProductionUnits { get; set; }
    public double HeatDemand { get; set; }
    public double ElectricityPrice { get; set; }

    public GeneticAlgorithm(List<ProductionUnit> productionUnits, double heatDemand, double electricityPrice, int seed)
    {
        ProductionUnits = productionUnits;
        HeatDemand = heatDemand;
        ElectricityPrice = electricityPrice;
        random = new Random(seed); // Set the seed for the random number generator
    }

    public Individual Run()
    {
        List<Individual> population = InitializePopulation();
        Individual bestIndividual = population[0];
        int generationsWithoutImprovement = 0;

        for (int generation = 0; generation < MaxGenerations; generation++)
        {
            List<Individual> newPopulation = new List<Individual>();

            // Perform crossover and mutation to create new population
            for (int i = 0; i < PopulationSize / 2; i++)
            {
                Individual parent1 = population[random.Next(PopulationSize)];
                Individual parent2 = population[random.Next(PopulationSize)];
                Individual offspring1 = Crossover(parent1, parent2);
                Individual offspring2 = Crossover(parent2, parent1);

                Mutate(offspring1);
                Mutate(offspring2);

                CalculateFitness(offspring1);
                CalculateFitness(offspring2);

                newPopulation.Add(offspring1);
                newPopulation.Add(offspring2);

                if (offspring1.Fitness > bestIndividual.Fitness)
                {
                    bestIndividual = offspring1;
                    generationsWithoutImprovement = 0;
                }

                if (offspring2.Fitness > bestIndividual.Fitness)
                {
                    bestIndividual = offspring2;
                    generationsWithoutImprovement = 0;
                }
            }

            population = newPopulation.OrderByDescending(ind => ind.Fitness).ToList();

            if (++generationsWithoutImprovement >= StagnationLimit)
            {
                Console.WriteLine($"Stopping early due to stagnation at generation {generation}");
                break;
            }

            Console.WriteLine($"Generation {generation}, Best Fitness: {bestIndividual.Fitness}");
        }

        return bestIndividual;
    }


    private List<Individual> InitializePopulation()
    {
        List<Individual> population = new List<Individual>();

        for (int i = 0; i < PopulationSize; i++)
        {
            Individual individual = new Individual(GeneLength);
            for (int j = 0; j < GeneLength; j++)
            {
                individual.Genes[j] = random.NextDouble(); // Use the seeded random instance
            }
            CalculateFitness(individual);
            population.Add(individual);
        }

        return population;
    }

    private void Mutate(Individual individual)
    {
        int geneIndex = random.Next(individual.Genes.Length);
        double mutation = (random.NextDouble() * 2 - 1) * MutationRate;
        individual.Genes[geneIndex] = Math.Clamp(individual.Genes[geneIndex] + mutation, 0, 1);

        // Recalculate heat production to guide mutation
        double totalHeat = 0;
        for (int i = 0; i < ProductionUnits.Count; i++)
        {
            totalHeat += ProductionUnits[i].MaxHeat * individual.Genes[i];
        }

        // Adjust genes to match the heat demand
        while (Math.Abs(totalHeat - HeatDemand) > 0.01)
        {
            int adjustmentIndex = random.Next(individual.Genes.Length);
            double adjustment = (HeatDemand - totalHeat) / ProductionUnits[adjustmentIndex].MaxHeat;
            individual.Genes[adjustmentIndex] = Math.Clamp(individual.Genes[adjustmentIndex] + adjustment, 0, 1);

            totalHeat = 0;
            for (int i = 0; i < ProductionUnits.Count; i++)
            {
                totalHeat += ProductionUnits[i].MaxHeat * individual.Genes[i];
            }
        }

        Console.WriteLine($"Mutated Gene {geneIndex} to {individual.Genes[geneIndex]}");
    }
    private void CalculateFitness(Individual individual)
    {
        individual.TotalHeat = 0;
        individual.TotalElectricity = 0;
        individual.Expenses = 0;
        individual.GasConsumption = 0;
        individual.OilConsumption = 0;
        individual.ElectricityConsumption = 0;
        individual.CO2 = 0;

        for (int i = 0; i < ProductionUnits.Count; i++)
        {
            double activationPercentage = individual.Genes[i];
            ProductionUnit unit = ProductionUnits[i];

            individual.ActivationPercentages[unit] = activationPercentage;

            double unitHeat = unit.MaxHeat * activationPercentage;
            double unitElectricity = unit.Name == "GM" ? unit.MaxElectricity * activationPercentage : 0;
            double unitCost = unit.ProductionCost * activationPercentage;
            double unitCO2 = unit.CO2Emissions * activationPercentage;
            double unitGasConsumption = unit.GasConsumption * activationPercentage;
            double unitOilConsumption = unit.OilConsumption * activationPercentage;
            double unitElectricityConsumption = unit.Name == "EK" ? -unit.MaxElectricity * activationPercentage : 0; // For electric boiler

            individual.TotalHeat += unitHeat;
            individual.TotalElectricity += unitElectricity;
            individual.Expenses += unitCost;
            individual.CO2 += unitCO2;
            individual.GasConsumption += unitGasConsumption;
            individual.OilConsumption += unitOilConsumption;
            individual.ElectricityConsumption += unitElectricityConsumption;
        }

        // Adjust the calculation of electricity costs
        if (individual.TotalElectricity > 0)
        {
            individual.Expenses -= individual.TotalElectricity * ElectricityPrice; // Revenue for produced electricity
        }
        else
        {
            individual.Expenses += individual.ElectricityConsumption * ElectricityPrice; // Cost for consumed electricity (since TotalElectricity is negative)
        }

        // Calculate penalties and incentives
        double heatPenalty = individual.TotalHeat < HeatDemand ? (HeatDemand - individual.TotalHeat) * 100000 : 0; // Strong penalty for not meeting heat demand
        double expensePenalty = individual.Expenses * 1000; // Very high penalty for expenses
        double co2Penalty = individual.CO2 * 0.01; // Reduced penalty for CO2 emissions

        // Adding a very high penalty for the use of OB unit
        double obPenalty = individual.ActivationPercentages[ProductionUnits.Find(p => p.Name == "OB")] * 1000000000000;

        // Incentives and penalties based on electricity price
        double gmIncentive = individual.ActivationPercentages[ProductionUnits.Find(p => p.Name == "GM")] * (ElectricityPrice > 900 ? -1000000 : 0); // High incentive if electricity price is high
        double ekPenalty = individual.ActivationPercentages[ProductionUnits.Find(p => p.Name == "EK")] * (ElectricityPrice > 900 ? 1000000 : 0); // High penalty if electricity price is high and EK is used

        // Adding a significant incentive for the use of GB to ensure heat demand is met
        double gbIncentive = individual.ActivationPercentages[ProductionUnits.Find(p => p.Name == "GB")] * -1000000;

        // Fitness function primarily aims to minimize expenses, with strong incentives for GM and GB when appropriate and penalties for OB and EK when prices are high
        individual.Fitness = 1.0 / (1 + heatPenalty + expensePenalty + co2Penalty + obPenalty + ekPenalty - gmIncentive - gbIncentive);

        Console.WriteLine($"Calculated Fitness: {individual.Fitness} for Heat: {individual.TotalHeat}, Expenses: {individual.Expenses}, CO2: {individual.CO2}, GM Incentive: {gmIncentive}, OB Penalty: {obPenalty}, GB Incentive: {gbIncentive}, EK Penalty: {ekPenalty}");
    }

    private Individual Crossover(Individual parent1, Individual parent2)
    {
        Individual offspring = new Individual(GeneLength);
        for (int i = 0; i < GeneLength; i++)
        {
            offspring.Genes[i] = (parent1.Genes[i] + parent2.Genes[i]) / 2.0;
        }
        CalculateFitness(offspring);
        return offspring;
    }
}
