public class ProductionUnit
{
    public string Name { get; set; }
    public double MaxHeat { get; set; }
    public double MaxElectricity { get; set; } // If applicable
    public double ProductionCost { get; set; }
    public double CO2Emissions { get; set; }
    public double GasConsumption { get; set; } // Only gas consumption for gas units
    public double OilConsumption { get; set; } // Only oil consumption for oil units
}
