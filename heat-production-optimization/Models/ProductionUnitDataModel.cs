using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace heat_production_optimization.Models
{
    public class ProductionUnitDataModel : IUnit
    {
        public Guid Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double MaxHeat { get; set; }
        public double MaxElectricity { get; set; }
        public double ProductionCost { get; set; }
        public double ProductionCostMWh { get; set; }
        public double CO2Emission { get; set; }
        public double CO2EmissionMWh { get; set; }
        public double GasConsumption { get; set; }
        public double OilConsumption { get; set; }
        public double PriceToHeatRatio { get; set; }
    }
}
