namespace heat_production_optimization.Models
{
    public class OptimizerUnitNamesDataModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public OptimizerUnitNamesDataModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }  
    }
}
