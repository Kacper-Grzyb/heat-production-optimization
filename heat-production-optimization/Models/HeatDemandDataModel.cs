

namespace heat_production_optimization.Models
{
    
    public class HeatDemandDataModel
    {
        public int Id { get; set; }
        public DateTime timeFrom { get; set; }
        public DateTime timeTo { get; set; }
        public double heatDemand { get; set; } // in MWh
        public double electricityPrice { get; set; } // in DKK/MWh (el)
    }
}
