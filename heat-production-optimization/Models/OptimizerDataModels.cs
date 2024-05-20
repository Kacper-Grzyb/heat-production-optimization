using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Models
{
	public class DateInterval
	{
		public Guid Id { get; set; }
		public DateTime TimeFrom { get; set; }
		public DateTime TimeTo { get; set; }
	}

	public class UnitActivationPercentage
	{
		public Guid Id { get; set; }
		public ProductionUnitDataModel Unit { get; set; }
		public double ActivationPercentage { get; set; }
	}

	public class UnitUsageDataModel
	{
		public Guid Id { get; set; }
		public DateInterval DateInterval { get; set; }
		public List<UnitActivationPercentage> ActivationPercentages { get; set; }
	}

	public class OptimizerResultsDataModel
	{
		public Guid Id { get; set; }
		public double TotalHeatProduction { get; set; } = 0.0;
		public double TotalElectricityProduction { get; set; } = 0.0;
		public double Expenses { get; set; } = 0.0;
		public double ConsumptionOfGas { get; set; } = 0.0;
		public double ConsumptionOfOil { get; set; } = 0.0;
		public double ConsumptionOfElectricity { get; set; } = 0.0;
		public double ProducedCO2 { get; set; } = 0.0;

	}
}
