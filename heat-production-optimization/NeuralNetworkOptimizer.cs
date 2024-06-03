using heat_production_optimization.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.ConditionalFormatting.Contracts;

namespace heat_production_optimization
{
    public class NeuralNetworkOptimizer : IOptimizer
    {
        private HeatDemandDataModel[] HeatDemandData { get; set; }
        private List<IUnit> ProductionUnits { get; set; }

        public double TotalHeatProduction { get; set; }
        public double TotalElectricityProduction { get; set; }
        public double Expenses { get; set; }
        public double ConsumptionOfGas { get; set; }
        public double ConsumptionOfOil { get; set; }
        public double ConsumptionOfElectricity { get; set; }
        public double ProducedCO2 { get; set; }
        public bool CanMeetHeatDemand { get; set; }
        public List<UnitUsageDataModel> unitUsages { get; set; }

        int seed = 43;

        public NeuralNetworkOptimizer(List<IUnit> productionUnits, DbSet<HeatDemandDataModel> heatDemandData)
        {
            ProductionUnits = productionUnits;
            HeatDemandData = heatDemandData.OrderBy(r => r.timeFrom).ToArray();

            TotalHeatProduction = 0;
            TotalElectricityProduction = 0;
            Expenses = 0;
            ConsumptionOfGas = 0;
            ConsumptionOfOil = 0;
            ConsumptionOfElectricity = 0;
            ProducedCO2 = 0;
            unitUsages = new List<UnitUsageDataModel>();
        }

        public void OptimizeHeatProduction(OptimizationOption option)
        {

            // Define penalties and incentives for each boiler unit
            Dictionary<string, double> unitPenalties = new Dictionary<string, double>
            {
                { "OB", 1000000000000 }, // Very high penalty for OB unit
            };

            Dictionary<string, double> unitIncentives = new Dictionary<string, double>
            {
                { "GB", -1000000 } // Significant incentive for GB unit to ensure heat demand is met
            };

            foreach (HeatDemandDataModel record in HeatDemandData)
            {
                GeneticAlgorithm ga = new GeneticAlgorithm(ProductionUnits, record.heatDemand, record.electricityPrice, seed, option, unitPenalties, unitIncentives);
                Individual bestSolution;

                switch (option)
                {
                    case OptimizationOption.Cost:
                        bestSolution = ga.Run();
                        break;
                    case OptimizationOption.Emission:
                        bestSolution = ga.Run();
                        break;
                    case OptimizationOption.Both:
                        bestSolution = ga.Run();
                        break;
                    default:
                        throw new Exception("Input not recognized");
                }

                CanMeetHeatDemand = ga.CanMeetHeatDemand;
                if (!CanMeetHeatDemand) return;

                TotalHeatProduction += bestSolution.TotalHeat;
                TotalElectricityProduction += bestSolution.TotalElectricity;
                Expenses += bestSolution.Expenses;
                ConsumptionOfGas += bestSolution.GasConsumption;
                ConsumptionOfOil += bestSolution.OilConsumption;
                ConsumptionOfElectricity += bestSolution.ElectricityConsumption;
                ProducedCO2 += bestSolution.CO2;

                // Saving the unit usage data

                List<UnitActivationPercentage> activations = new List<UnitActivationPercentage>();
                foreach (var kvp in bestSolution.ActivationPercentages)
                {
                    //Console.WriteLine($"{kvp.Key.Name} Activation: {kvp.Value * 100}%");
                    var activation = new UnitActivationPercentage()
                    {
                        Id = Guid.NewGuid(),
                        Unit = (ProductionUnitDataModel)kvp.Key,
                        ActivationPercentage = kvp.Value
                    };
                    activations.Add(activation);
                }

                var usage = new UnitUsageDataModel()
                {
                    Id = Guid.NewGuid(),
                    DateInterval = new DateInterval()
                    {
                        Id = Guid.NewGuid(),
                        TimeFrom = record.timeFrom,
                        TimeTo = record.timeTo,
                    },
                    ActivationPercentages = activations
                };

                unitUsages.Add(usage);
            }
        }
    }

    public class GeneticAlgorithm
    {
        private static Random random = new Random();
        private const double MutationRate = 0.01;
        private const int GeneLength = 4;
        private const int PopulationSize = 200;
        private const int MaxGenerations = 1000;
        private const int StagnationLimit = 100;
        

        public List<IUnit> ProductionUnits { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }
        public OptimizationOption Option { get; set; }
        public Dictionary<string, double> UnitPenalties { get; set; }
        public Dictionary<string, double> UnitIncentives { get; set; }
        public bool CanMeetHeatDemand { get; set; }

        public GeneticAlgorithm(List<IUnit> productionUnits, double heatDemand, double electricityPrice, int seed, OptimizationOption option, Dictionary<string, double> unitPenalties, Dictionary<string, double> unitIncentives)
        {
            ProductionUnits = productionUnits;
            HeatDemand = heatDemand;
            ElectricityPrice = electricityPrice;
            random = new Random(seed); // Set the seed for the random number generator
            Option = option;
            UnitPenalties = unitPenalties;
            UnitIncentives = unitIncentives;
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
                    //Console.WriteLine($"Stopping early due to stagnation at generation {generation}");
                    break;
                }

                //Console.WriteLine($"Generation {generation}, Best Fitness: {bestIndividual.Fitness}");
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

            //Console.WriteLine($"Mutated Gene {geneIndex} to {individual.Genes[geneIndex]}");
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
                IUnit unit = ProductionUnits[i];

                individual.ActivationPercentages[unit] = activationPercentage;

                double unitHeat = unit.MaxHeat * activationPercentage;
                double unitElectricity = unit.MaxElectricity >= 0 ? unit.MaxElectricity * activationPercentage : 0;
                double unitCost = unit.ProductionCostMWh * activationPercentage;
                double unitCO2 = unit.CO2EmissionMWh * activationPercentage;
                double unitGasConsumption = unit.GasConsumption * activationPercentage;
                double unitOilConsumption = unit.OilConsumption * activationPercentage;
                double unitElectricityConsumption = unit.MaxElectricity <= 0 ? -unit.MaxElectricity * activationPercentage : 0; // For electric boiler

                individual.TotalHeat += unitHeat;
                individual.TotalElectricity += unitElectricity;
                individual.Expenses += unitCost;
                individual.CO2 += unitCO2;
                individual.GasConsumption += unitGasConsumption;
                individual.OilConsumption += unitOilConsumption;
                individual.ElectricityConsumption += unitElectricityConsumption;
            }

            // Safety check for meeting heat demand
            if (individual.TotalHeat < HeatDemand)
            {
                CanMeetHeatDemand = false;
            }
            else
            {
                CanMeetHeatDemand = true;
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

            double fitness;
            double heatPenalty = individual.TotalHeat < HeatDemand ? (HeatDemand - individual.TotalHeat) * 100000 : 0; // Strong penalty for not meeting heat demand

            if (Option == OptimizationOption.Cost)
            {
                // Calculate fitness based only on expenses
                double expensePenalty = individual.Expenses * 1000;

                double totalPenalty = 0;
                foreach (var unit in ProductionUnits)
                {
                    if (UnitPenalties.ContainsKey(unit.Alias))
                    {
                        totalPenalty += individual.ActivationPercentages[unit] * UnitPenalties[unit.Alias];
                    }
                }

                double totalIncentive = 0;
                foreach (var unit in ProductionUnits)
                {
                    if (UnitIncentives.ContainsKey(unit.Alias))
                    {
                        totalIncentive += individual.ActivationPercentages[unit] * UnitIncentives[unit.Alias];
                    }
                }

                if (ElectricityPrice > 900)
                {
                    if (individual.ActivationPercentages.ContainsKey(ProductionUnits.Find(p => p.Alias == "GM")))
                    {
                        totalIncentive += individual.ActivationPercentages[ProductionUnits.Find(p => p.Alias == "GM")] * -1000000;
                    }

                    if (individual.ActivationPercentages.ContainsKey(ProductionUnits.Find(p => p.Alias == "EK")))
                    {
                        totalPenalty += individual.ActivationPercentages[ProductionUnits.Find(p => p.Alias == "EK")] * 1000000;
                    }
                }

                fitness = 1.0 / (1 + heatPenalty + expensePenalty + totalPenalty - totalIncentive);
            }
            else if (Option == OptimizationOption.Emission)
            {
                // Calculate fitness based only on CO2 emissions
                double co2Penalty = individual.CO2 * 1000;

                double totalPenalty = 0;
                foreach (var unit in ProductionUnits)
                {
                    if (UnitPenalties.ContainsKey(unit.Alias))
                    {
                        totalPenalty += individual.ActivationPercentages[unit] * UnitPenalties[unit.Alias];
                    }
                }

                double totalIncentive = 0;
                foreach (var unit in ProductionUnits)
                {
                    if (UnitIncentives.ContainsKey(unit.Alias))
                    {
                        totalIncentive += individual.ActivationPercentages[unit] * UnitIncentives[unit.Alias];
                    }
                }

                if (ElectricityPrice > 900)
                {
                    if (individual.ActivationPercentages.ContainsKey(ProductionUnits.Find(p => p.Alias == "GM")))
                    {
                        totalIncentive += individual.ActivationPercentages[ProductionUnits.Find(p => p.Alias == "GM")] * -1000000;
                    }

                    if (individual.ActivationPercentages.ContainsKey(ProductionUnits.Find(p => p.Alias == "EK")))
                    {
                        totalPenalty += individual.ActivationPercentages[ProductionUnits.Find(p => p.Alias == "EK")] * 1000000;
                    }
                }

                fitness = 1.0 / (1 + heatPenalty + co2Penalty + totalPenalty - totalIncentive);
            }
            else if (Option == OptimizationOption.Both)
            {
                // Calculate fitness based on both expenses and CO2 emissions
                double expensePenalty = individual.Expenses * 1000;
                double co2Penalty = individual.CO2 * 1000;

                double totalPenalty = 0;
                foreach (var unit in ProductionUnits)
                {
                    if (UnitPenalties.ContainsKey(unit.Alias))
                    {
                        totalPenalty += individual.ActivationPercentages[unit] * UnitPenalties[unit.Alias];
                    }
                }

                double totalIncentive = 0;
                foreach (var unit in ProductionUnits)
                {
                    if (UnitIncentives.ContainsKey(unit.Alias))
                    {
                        totalIncentive += individual.ActivationPercentages[unit] * UnitIncentives[unit.Alias];
                    }
                }

                if (ElectricityPrice > 900)
                {
                    if (individual.ActivationPercentages.ContainsKey(ProductionUnits.Find(p => p.Alias == "GM")))
                    {
                        totalIncentive += individual.ActivationPercentages[ProductionUnits.Find(p => p.Alias == "GM")] * -1000000;
                    }

                    if (individual.ActivationPercentages.ContainsKey(ProductionUnits.Find(p => p.Alias == "EK")))
                    {
                        totalPenalty += individual.ActivationPercentages[ProductionUnits.Find(p => p.Alias == "EK")] * 1000000;
                    }
                }

                fitness = 1.0 / (1 + heatPenalty + expensePenalty + co2Penalty + totalPenalty - totalIncentive);
            }
            else
            {
                throw new Exception("Invalid optimization option selected.");
            }

            individual.Fitness = fitness;
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


    public class Solution
    {
        public double Heat { get; set; }
        public double Electricity { get; set; }
        public double Cost { get; set; }
        public double CO2 { get; set; }
        public double Fitness { get; set; }

        private static Random rand = new Random();

        // Constructor to initialize the solution with random values
        public Solution()
        {
            Heat = rand.NextDouble() * 100;
            Electricity = rand.NextDouble() * 100;
            CalculateAttributes();
            CalculateFitness();
        }

        // Method to calculate cost and CO2 based on heat and electricity
        public void CalculateAttributes()
        {
            Cost = 0.6 * Heat + 0.4 * Electricity;
            CO2 = 0.5 * Heat + 0.3 * Electricity;
        }

        // Method to calculate fitness based on cost and CO2
        public void CalculateFitness()
        {
            Fitness = 1 / (Cost + CO2); // Inverse of cost + CO2 as fitness
        }

        // Method to mutate the solution
        public void Mutate()
        {
            Heat += (rand.NextDouble() - 0.5) * 20; // Randomly alter heat
            Electricity += (rand.NextDouble() - 0.5) * 20; // Randomly alter electricity
            CalculateAttributes();
            CalculateFitness();
        }

        // Method to crossover two solutions
        public static Solution Crossover(Solution parent1, Solution parent2)
        {
            Solution child = new Solution
            {
                Heat = (parent1.Heat + parent2.Heat) / 2,
                Electricity = (parent1.Electricity + parent2.Electricity) / 2
            };
            child.CalculateAttributes();
            child.CalculateFitness();
            return child;
        }
    }

    public class Individual
    {
        public double[] Genes { get; set; }
        public double Fitness { get; set; }
        public double TotalHeat { get; set; }
        public double TotalElectricity { get; set; }
        public double Expenses { get; set; }
        public double GasConsumption { get; set; }
        public double OilConsumption { get; set; }
        public double ElectricityConsumption { get; set; }
        public double CO2 { get; set; }
        public Dictionary<IUnit, double> ActivationPercentages { get; set; } = new Dictionary<IUnit, double>();



        public Individual(int geneLength)
        {
            Genes = new double[geneLength];
            RandomizeGenes();
        }
        private void RandomizeGenes()
        {
            Random random = new Random();
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = random.NextDouble();
            }
        }
    }
}
