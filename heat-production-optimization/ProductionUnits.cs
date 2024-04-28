using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace heat_production_optimization
{
    public interface IUnit
    {
        public string? Alias { get; }
        public string? Name { get;}
        public double? MaxHeat { get;}
        public double? MaxElectricity { get; }
        public int? ProductionCost { get;}
        public int? CO2Emission { get;}
        public double? GasConsumption { get; }
        public double? OilConsumption { get; }
    }

    public class GasBoiler : IUnit
    {
        public string? Alias { get; }
        public string? Name { get;}
        public double? MaxHeat { get;}
        public int? ProductionCost { get;}
        public int? CO2Emission { get;}
        public double? GasConsumption { get;}

        public double? OilConsumption { get; } = null;
        public double? MaxElectricity { get; } = null;

        public GasBoiler(double maxHeat, int productionCost, int cO2Emission, double gasConsumption, string alias = "GB", string name = "Gas Boiler")
        {
            Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
            GasConsumption = gasConsumption;
        } 
    }

    public class OilBoiler : IUnit
    {
        public string? Alias { get; }
        public string? Name { get;}
        public double? MaxHeat { get;}
        public int? ProductionCost { get;}
        public int? CO2Emission { get;}
        public double? OilConsumption { get;}

        public double? GasConsumption { get; } = null;
        public double? MaxElectricity { get; } = null;

        public OilBoiler(double maxHeat, int productionCost, int cO2Emission, double oilConsumption, string alias = "OB", string name = "Oil Boiler")
        {
            Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
            OilConsumption = oilConsumption;
        } 
    }

    public class GasMotor : IUnit
    {
        public string? Alias { get; }
        public string? Name { get;}
        public double? MaxHeat { get;}
        public double? MaxElectricity { get;}
        public int? ProductionCost { get;}
        public int? CO2Emission { get;}
        public double? GasConsumption { get;}

        public double? OilConsumption { get; } = null;

        public GasMotor(double maxHeat, double maxElectricity, int productionCost, int cO2Emission, double gasConsumption, string alias = "GM", string name = "Gas Motor")
        {
            Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            MaxElectricity = maxElectricity;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
            GasConsumption = gasConsumption;
        }   
    }

    public class ElectricBoiler : IUnit
    {
        public string? Alias { get; }
        public string? Name { get;}
        public double? MaxHeat { get;}
        public double? MaxElectricity { get;}
        public int? ProductionCost { get;}
        public int? CO2Emission { get;}

        public double? GasConsumption { get; } = null;
        public double? OilConsumption { get; } = null;

        public ElectricBoiler(double maxHeat, double maxElectricity, int productionCost, int cO2Emission, string alias = "EK", string name = "Electric Boiler")
        {
            Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            MaxElectricity = maxElectricity;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
        }
    }

    public class CustomBoiler : IUnit
    {
		public string? Alias { get; }
		public string? Name { get; }
		public double? MaxHeat { get; }
		public double? MaxElectricity { get; }
		public int? ProductionCost { get; }
		public int? CO2Emission { get; }
		public double? GasConsumption { get; }
		public double? OilConsumption { get; }

        public CustomBoiler(string? alias, string? name, double? maxHeat = null, double? maxElectricity = null, int? productionCost = null, int? cO2Emission = null, double? gasConsumption = null, double? oilConsumption = null)
		{
			Alias = alias;
			Name = name;
			MaxHeat = maxHeat;
			MaxElectricity = maxElectricity;
			ProductionCost = productionCost;
			CO2Emission = cO2Emission;
			GasConsumption = gasConsumption;
			OilConsumption = oilConsumption;
		}
	}


}