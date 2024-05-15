using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace heat_production_optimization.Models
{
    public class SourceDataDbContext : DbContext
    {
        // TODO add user input to save here
        public DbSet<HeatDemandDataModel> HeatDemandData { get; set; }
        public DbSet<IUnit> productionUnits { get; set; }
        public DbSet<IUnit> productionUnitsForOptimization { get; set; }
        public string loadedDataPath { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;
        public IUnit gasBoiler { 
            get
            {
                return productionUnits.Find("GB");
            }
        }
        public IUnit oilBoiler { 
            get
            {
                return productionUnits.Find("OB");
            }
        }
        public IUnit gasMotor { 
            get
            {
                return productionUnits.Find("GM");
            } 
        }
        public IUnit electricBoiler { 
            get
            {
                return productionUnits.Find("EB");
            } 
        }

        public SourceDataDbContext()
        {
            if(productionUnits!=null)
            {
                productionUnits.Add(new GasBoiler(5, 500, 215, 1.1, "GB", "Gas Boiler"));
                productionUnits.Add(new OilBoiler(4, 700, 265, 1.2, "OB", "Oil Boiler"));
                productionUnits.Add(new GasMotor(3.6, 2.7, 1100, 640, 1.9, "GM", "Gas Motor"));
                productionUnits.Add(new ElectricBoiler(8, -8, 50, 0, "EK", "Electric Boiler"));
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("HeatManagementDb");

        }

        public bool IsDataLoaded()
        {
            Console.WriteLine($"Loaded data path {loadedDataPath}");
            if (HeatDemandData == null) return false;
            else return HeatDemandData.Count() > 0;
        }
    }
}
