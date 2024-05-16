using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace heat_production_optimization.Models
{
    public class SourceDataDbContext : DbContext
    {
        // TODO add user input to save here
        public DbSet<HeatDemandDataModel> HeatDemandData { get; set; }
        public string loadedDataPath { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;
        public IUnit gasBoiler { get; set; }
        public IUnit oilBoiler { get; set; }
        public IUnit gasMotor { get; set; }
        public IUnit electricBoiler { get; set; }
        public List<IUnit> productionUnits { get; set; }
        public List<IUnit> optimizerProductionUnits { get; set; } = new List<IUnit>();

        public SourceDataDbContext()
        {
			gasBoiler = new GasBoiler(5, 500, 215, 1.1, "GB", "Gas Boiler");
			oilBoiler = new OilBoiler(4, 700, 265, 1.2, "OB", "Oil Boiler");
			gasMotor = new GasMotor(3.6, 2.7, 1100, 640, 1.9, "GM", "Gas Motor");
			electricBoiler = new ElectricBoiler(8, -8, 50, 0, "EK", "Electric Boiler");
            productionUnits = new List<IUnit>() { gasBoiler, oilBoiler, gasMotor, electricBoiler };
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("HeatManagementDb");
		}

        public bool IsDataLoaded()
        {
            if (HeatDemandData == null) return false;
            else return HeatDemandData.Count() > 0;
        }
    }
}
