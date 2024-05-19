using heat_production_optimization.Models;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace heat_production_optimization
{
    public interface IUnit
    {
        public Guid Id { get; set; }
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
		public GasBoiler(Guid id, double maxHeat, double productionCost, double cO2Emission, double gasConsumption, string alias = "GB", string name = "Gas Boiler")
        {
            Id = id;
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

		public OilBoiler(Guid id, double maxHeat, double productionCost, double cO2Emission, double oilConsumption, string alias = "OB", string name = "Oil Boiler")
        {
            Id = id;
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

		public GasMotor(Guid id, double maxHeat, double maxElectricity, double productionCost, double cO2Emission, double gasConsumption, string alias = "GM", string name = "Gas Motor")
        {
            Id = id;
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

        public ElectricBoiler(Guid id, double maxHeat, double maxElectricity, double productionCost, double cO2Emission, string alias = "EK", string name = "Electric Boiler")
        {
            Id = id;
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

		public CustomUnit(Guid id,string alias, string name, double maxHeat = 0, double maxElectricity = 0, double productionCost = 0, double cO2Emission = 0, double gasConsumption = 0, double oilConsumption = 0)
		{
            Id = id;
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