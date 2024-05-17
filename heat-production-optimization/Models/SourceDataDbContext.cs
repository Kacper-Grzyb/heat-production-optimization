using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Principal;

namespace heat_production_optimization.Models
{
    public class SourceDataDbContext : DbContext
    {
        // TODO add user input to save here
        public DbSet<HeatDemandDataModel> HeatDemandData { get; set; }
        public DbSet<ProductionUnitDataModel> productionUnits { get; set; }
        public DbSet<ProductionUnitDataModel> productionUnitsForOptimization { get; set; }
        public DbSet<UiMessagesDataModel> uiMessages { get; set; }
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
                IUnit? findResult = productionUnits.Find("GB");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'GB'!");
            }
        }
        public IUnit oilBoiler 
        { 
            get
            {
                IUnit? findResult = productionUnits.Find("OB");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'OB'!");
            }
        }
        public IUnit gasMotor 
        { 
            get
            {
                IUnit? findResult = productionUnits.Find("GM");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'GM'!");
            } 
        }
        public IUnit electricBoiler 
        { 
            get
            {
                IUnit? findResult = productionUnits.Find("EK");
                if (findResult != null) return findResult;
                else throw new Exception("Could not find a value in the productionUnits table with the key 'EK'!");
            } 
        }

        public SourceDataDbContext()
        {
            if(productionUnits!=null && productionUnits.Count() == 0)
            {
                // TODO Check why this does not save the gasboiler correctly, instead creates an empty field
                productionUnits.Add(new GasBoiler(5, 500, 215, 1.1, "GB", "Gas Boiler"));
                productionUnits.Add(new OilBoiler(4, 700, 265, 1.2, "OB", "Oil Boiler"));
                productionUnits.Add(new GasMotor(3.6, 2.7, 1100, 640, 1.9, "GM", "Gas Motor"));
                productionUnits.Add(new ElectricBoiler(8, -8, 50, 0, "EK", "Electric Boiler"));

                productionUnitsForOptimization = productionUnits;
            }

            if(uiMessages!=null && uiMessages.Count() == 0)
            {
                uiMessages.Add(new UiMessagesDataModel(MessageType.DataUploadError, string.Empty));
                uiMessages.Add(new UiMessagesDataModel(MessageType.DataUploadPath, string.Empty));
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
    }
}
