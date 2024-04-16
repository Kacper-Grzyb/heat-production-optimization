using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace heat_production_optimization.Models
{
    public class SourceDataDbContext : DbContext
    {
        // TODO add user input to save here
        public DbSet<HeatDemandDataModel> HeatDemandData { get; set; }
        public string loadedDataPath { get; set; }
        public string errorMessage { get; set; }
        public GasBoiler gasBoiler { get; set; }
        public OilBoiler oilBoiler { get; set; }
        public GasMotor gasMotor { get; set; }
        public ElectricBoiler electricBoiler { get; set; }

        public SourceDataDbContext()
        {
			gasBoiler = new GasBoiler("GB", 5, 500, 215, 1.1);
			oilBoiler = new OilBoiler("OB", 4, 700, 265, 1.2);
			gasMotor = new GasMotor("GM", 3.6, 2.4, 1100, 640, 1.9);
			electricBoiler = new ElectricBoiler("EK", 8, -8, 50, 0);
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
