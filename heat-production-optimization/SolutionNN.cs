using System;

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
