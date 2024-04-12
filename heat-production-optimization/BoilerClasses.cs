using Microsoft.AspNetCore.SignalR;

namespace heat_production_optimization
{
    public interface IBoiler
    {
        public string? Name { get;}
        public double MaxHeat { get;}
        public int ProductionCost { get;}
        public int CO2Emission { get;}
    }

    public class GasBoiler : IBoiler
    {
        public string? Name { get;}
        public double MaxHeat { get;}
        public int ProductionCost { get;}
        public int CO2Emission { get;}
        public double GasConsumption { get;}

        public GasBoiler(string name, double maxHeat, int productionCost, int cO2Emission, double gasConsumption)
        {
            Name = name;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
            GasConsumption = gasConsumption;
        } 
    }

    public class OilBoiler : IBoiler
    {
        public string? Name { get;}
        public double MaxHeat { get;}
        public int ProductionCost { get;}
        public int CO2Emission { get;}
        public double OilConsumption { get;}

        public OilBoiler(string name, double maxHeat, int productionCost, int cO2Emission, double oilConsumption)
        {
            Name = name;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
            OilConsumption = oilConsumption;
        } 

    }

    public class GasMotor : IBoiler
    {
        public string? Name { get;}
        public double MaxHeat { get;}
        public double MaxEletricity { get;}
        public int ProductionCost { get;}
        public int CO2Emission { get;}
        public double GasConsumption { get;}

        public GasMotor(string name, double maxHeat, double maxElectricity, int productionCost, int cO2Emission, double gasConsumption)
        {
            Name = name;
            MaxHeat = maxHeat;
            MaxEletricity = maxElectricity;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
            GasConsumption = gasConsumption;
        }   
    }

    public class ElectricBoiler : IBoiler
    {
        public string? Name { get;}
        public double MaxHeat { get;}
        public double MaxEletricity { get;}
        public int ProductionCost { get;}
        public int CO2Emission { get;}

        public ElectricBoiler(string name, double maxHeat, double maxElectricity, int productionCost, int cO2Emission)
        {
            Name = name;
            MaxHeat = maxHeat;
            MaxEletricity = maxElectricity;
            ProductionCost = productionCost;
            CO2Emission = cO2Emission;
        }
    }


}