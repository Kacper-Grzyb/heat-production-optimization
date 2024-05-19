using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Principal;

namespace heat_production_optimization.Models
{
    public class SourceDataDbContext : DbContext
    {
        public DbSet<HeatDemandDataModel> HeatDemandData { get; set; }
        public DbSet<ProductionUnitDataModel> productionUnits { get; set; }
        public DbSet<OptimizerUnitNamesDataModel> productionUnitNamesForOptimization { get; set; }
        public DbSet<UiMessagesDataModel> uiMessages { get; set; }
        public DbSet<UnitUsageDataModel> unitUsage { get; set; }
        public DbSet<OptimizerResultsDataModel> optimizerResults { get; set; }
        public string loadedDataPath 
        { 
            get
            {
                string? findResult = uiMessages.Find(MessageType.DataUploadPath)?.Message;
                if (findResult != null) return findResult;
                else throw new Exception($"Could not find a value in the uiMessages table with the key {MessageType.DataUploadPath}!");
            }
            set
            {
                if (uiMessages.Find(MessageType.DataUploadPath)?.Message != null)
                {
                    uiMessages.Find(MessageType.DataUploadPath).Message = value;
                }
                else throw new Exception($"Could not find a value in the uiMessages table with the key {MessageType.DataUploadPath}!");
            }            
        }
        public string errorMessage 
        { 
            get 
            {
                string? findResult = uiMessages.Find(MessageType.DataUploadError)?.Message;
                if (findResult != null) return findResult;
                else throw new Exception($"Could not find a value in the uiMessages table with the key {MessageType.DataUploadError}!");
            }
            set
            {
                if (uiMessages.Find(MessageType.DataUploadError)?.Message != null)
                {
                    uiMessages.Find(MessageType.DataUploadError).Message = value;
                }
                else throw new Exception($"Could not find a value in the uiMessages table with the key {MessageType.DataUploadError}!");
            }
        }

        public IUnit gasBoiler 
        { 
            get
            {
                IUnit? findResult = productionUnits.FirstOrDefault(u => u.Alias == "GB");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'GB'!");
            }
        }
        public IUnit oilBoiler 
        { 
            get
            {
                IUnit? findResult = productionUnits.FirstOrDefault(u => u.Alias == "OB");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'OB'!");
            }
        }
        public IUnit gasMotor 
        { 
            get
            {
                IUnit? findResult = productionUnits.FirstOrDefault(u => u.Alias == "GM");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'GM'!");
            } 
        }
        public IUnit electricBoiler 
        { 
            get
            {
                IUnit? findResult = productionUnits.FirstOrDefault(u => u.Alias == "EK");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'EK'!");
            } 
        }

        public SourceDataDbContext()
        {
            if (productionUnits != null && productionUnits.Count() == 0)
            {
                productionUnits.Add(new GasBoiler(Guid.NewGuid(), 5, 500, 215, 1.1, "GB", "Gas Boiler"));
                productionUnits.Add(new OilBoiler(Guid.NewGuid(), 4, 700, 265, 1.2, "OB", "Oil Boiler"));
                productionUnits.Add(new GasMotor(Guid.NewGuid(), 3.6, 2.7, 1100, 640, 1.9, "GM", "Gas Motor"));
                productionUnits.Add(new ElectricBoiler(Guid.NewGuid(), 8, -8, 50, 0, "EK", "Electric Boiler"));
            }

            if(uiMessages != null && uiMessages.Count() == 0)
            {
                uiMessages.Add(new UiMessagesDataModel(MessageType.DataUploadError, string.Empty));
                uiMessages.Add(new UiMessagesDataModel(MessageType.DataUploadPath, string.Empty));
                uiMessages.Add(new UiMessagesDataModel(MessageType.OptimizerError, string.Empty));
            }

            SaveChanges();
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

        public OptimizedResults? GetOptimizedResults(string selectedUnit)
        {
            
            var unit = productionUnits.FirstOrDefault(u => u.Name == selectedUnit);
            if (unit != null)
            {
                return new OptimizedResults
                {
                    TotalHeatProduced = unit.MaxHeat,
                    TotalElectricityProduced = unit.MaxElectricity,
                    TotalExpenses = unit.ProductionCost * unit.MaxHeat,
                    TotalGasConsumption = unit.GasConsumption,
                    TotalOilConsumption = unit.OilConsumption,
                    TotalCO2Emission = unit.CO2Emission * unit.MaxHeat
                    
                };
            }
            else
            {
                return null;
            }

        }

    }

    public class OptimizedResults
    {
        public double TotalHeatProduced { get; set; }
        public double TotalElectricityProduced { get; set; }
        public double TotalExpenses { get; set; }
        public double TotalGasConsumption { get; set; }
        public double TotalOilConsumption { get; set; }
        public double TotalCO2Emission { get; set; }

    }
}