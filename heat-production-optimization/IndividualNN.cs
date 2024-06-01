using System;
using System.Collections.Generic;

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
    public Dictionary<ProductionUnit, double> ActivationPercentages { get; set; } = new Dictionary<ProductionUnit, double>();



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
