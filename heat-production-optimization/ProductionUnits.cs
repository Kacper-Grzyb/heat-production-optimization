using heat_production_optimization.Models;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace heat_production_optimization
{
    public interface IUnit
    {
        public string Alias { get; }
        public string Name { get;}
        public double MaxHeat { get;}
        public double MaxElectricity { get; }
        public double ProductionCost { get; }
        public double ProductionCostMWh { get;}
        public double CO2Emission { get; }
        public double CO2EmissionMWh { get;}
        public double GasConsumption { get; }
        public double OilConsumption { get; }

        public double PriceToHeatRatio { get; set; }
    }

    public class GasBoiler : ProductionUnitDataModel, IUnit
    {
  //      public string Alias { get; }
  //      public string Name { get;}
  //      public double MaxHeat { get;}
  //      public double ProductionCost { get;}
		//public double ProductionCostMWh { get; }
		//public double CO2Emission { get;}
		//public double CO2EmissionMWh { get; }
		//public double GasConsumption { get;}
		//public double PriceToHeatRatio { get; set; }

  //      public double OilConsumption { get; } = 0;
  //      public double MaxElectricity { get; } = 0;

		public GasBoiler(double maxHeat, int productionCost, int cO2Emission, double gasConsumption, string alias = "GB", string name = "Gas Boiler")
        {
			if (maxHeat == 0) maxHeat = 0.01;
			PriceToHeatRatio = productionCost / maxHeat;
			Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            ProductionCostMWh = productionCost;
            ProductionCost = productionCost * maxHeat;
            CO2EmissionMWh = cO2Emission;
            CO2Emission = cO2Emission * maxHeat;
            GasConsumption = gasConsumption;
		} 
    }

    public class OilBoiler : ProductionUnitDataModel, IUnit
    {
  //      public string Alias { get; }
  //      public string Name { get;}
  //      public double MaxHeat { get;}
		//public double ProductionCost { get;}
		//public double ProductionCostMWh { get; }
  //      public double CO2Emission { get;}
  //      public double CO2EmissionMWh { get;}
  //      public double OilConsumption { get;}
		//public double PriceToHeatRatio { get; set; }

  //      public double GasConsumption { get; } = 0;
  //      public double MaxElectricity { get; } = 0;

		public OilBoiler(double maxHeat, int productionCost, int cO2Emission, double oilConsumption, string alias = "OB", string name = "Oil Boiler")
        {
			if (maxHeat == 0) maxHeat = 0.01;
			PriceToHeatRatio = productionCost / maxHeat;
			Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            ProductionCostMWh = productionCost;
            ProductionCost = productionCost * maxHeat;
			CO2EmissionMWh = cO2Emission;
			CO2Emission = cO2Emission * maxHeat;
			OilConsumption = oilConsumption;
		} 
    }

    public class GasMotor : ProductionUnitDataModel, IUnit
    {
  //      public string Alias { get; }
  //      public string Name { get;}
  //      public double MaxHeat { get;}
  //      public double MaxElectricity { get;}
  //      public double ProductionCost { get;}
		//public double ProductionCostMWh { get; }
		//public double CO2Emission { get;}
  //      public double CO2EmissionMWh { get; } 
  //      public double GasConsumption { get;}
		//public double PriceToHeatRatio { get; set; }

  //      public double OilConsumption { get; } = 0;

		public GasMotor(double maxHeat, double maxElectricity, int productionCost, int cO2Emission, double gasConsumption, string alias = "GM", string name = "Gas Motor")
        {
			if (maxHeat == 0) maxHeat = 0.01;
			PriceToHeatRatio = productionCost / maxHeat;
			Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            MaxElectricity = maxElectricity;
            ProductionCost = productionCost * maxHeat;
            ProductionCostMWh = productionCost;
			CO2Emission = cO2Emission * maxHeat;
			CO2EmissionMWh = cO2Emission;
            GasConsumption = gasConsumption;
		}   
    }

    public class ElectricBoiler : ProductionUnitDataModel, IUnit
    {
  //      public string Alias { get; }
  //      public string Name { get;}
  //      public double MaxHeat { get;}
  //      public double MaxElectricity { get;}
  //      public double ProductionCost { get;}
		//public double ProductionCostMWh { get; }
		//public double CO2Emission { get;}
  //      public double CO2EmissionMWh { get; }
		//public double PriceToHeatRatio { get; set; }

		//public double GasConsumption { get; } = 0;
  //      public double OilConsumption { get; } = 0;

        public ElectricBoiler(double maxHeat, double maxElectricity, int productionCost, int cO2Emission, string alias = "EK", string name = "Electric Boiler")
        {
			if (maxHeat == 0) maxHeat = 0.01;
			PriceToHeatRatio = productionCost / maxHeat;
			Alias = alias;
            Name = name;
            MaxHeat = maxHeat;
            MaxElectricity = maxElectricity;
            ProductionCost = productionCost * maxHeat;
            ProductionCostMWh = productionCost;
			CO2Emission = cO2Emission * maxHeat;
			CO2EmissionMWh = cO2Emission;
		}
    }

    public class CustomUnit : ProductionUnitDataModel, IUnit
    {
		//public string Alias { get; }
		//public string Name { get; }
		//public double MaxHeat { get; }
		//public double MaxElectricity { get; }
		//public double ProductionCost { get; }
		//public double ProductionCostMWh { get; }
		//public double CO2Emission { get; }
  //      public double CO2EmissionMWh { get; }
		//public double GasConsumption { get; }
		//public double OilConsumption { get; }
		//public double PriceToHeatRatio { get; set; }

		public CustomUnit(string alias, string name, double maxHeat = 0, double maxElectricity = 0, int productionCost = 0, int cO2Emission = 0, double gasConsumption = 0, double oilConsumption = 0)
		{
            if(maxHeat == 0) maxHeat = 0.01;
			PriceToHeatRatio = productionCost / maxHeat;
			Alias = alias;
			Name = name;
			MaxHeat = maxHeat;
			MaxElectricity = maxElectricity;
			ProductionCost = productionCost * maxHeat;
            ProductionCostMWh = productionCost;
			CO2Emission = cO2Emission * maxHeat;
			CO2EmissionMWh = cO2Emission;
			GasConsumption = gasConsumption;
			OilConsumption = oilConsumption;
		}
	}
}