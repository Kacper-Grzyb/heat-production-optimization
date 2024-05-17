using heat_production_optimization;
using CsvHelper;
using System.Globalization;
using heat_production_optimization.Models;



namespace heat_production_optimization
{
    public class HourlyOptimization
    {
        
        public DateTime timeFrom { get; set; }
        public DateTime timeTo { get; set; }
        public static List<HourlyOptimization> HourlyOptimizations { get; set; } = new List<HourlyOptimization>();
        public Dictionary<IUnit, double> properties { get; private set; } = new Dictionary<IUnit, double>();

        public double this[IUnit unitName]
        {
            get
            {
                if (properties.ContainsKey(unitName))
                {
                    return properties[unitName];
                }
                throw new KeyNotFoundException($"Property '{unitName.Name}' not found.");
            }
            set
            {
                properties[unitName] = value;
            }
        }
    }

    public class HourlyOptimizationCsv
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public Dictionary<string, double> Properties { get; set; }

        public HourlyOptimizationCsv()
        {
            Properties = new Dictionary<string, double>();
        }
    }
    public class SaveToCSV
    {
        public static int Nr { get; set; } = 1;

        public static void SaveOptimization()
        {
            if (HourlyOptimization.HourlyOptimizations == null || !HourlyOptimization.HourlyOptimizations.Any())
            {
                Console.WriteLine("No data available to write to CSV.");
                return;
            }

            string fileName = $"{Nr}_optimization.csv";
            Nr++;

            var csvData = HourlyOptimization.HourlyOptimizations.Select(ho => new HourlyOptimizationCsv
            {
                TimeFrom = ho.timeFrom,
                TimeTo = ho.timeTo,
                Properties = ho.properties.ToDictionary(p => p.Key.Name, p => p.Value)
            }).ToList();

            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
            {
                // Write header
                csv.WriteField("TimeFrom");
                csv.WriteField("TimeTo");

                // Write dynamic property headers
                var allUnitNames = csvData.SelectMany(x => x.Properties.Keys).Distinct().OrderBy(x => x).ToList();
                foreach (var unitName in allUnitNames)
                {
                    csv.WriteField(unitName);
                }
                csv.NextRecord();

                // Write records
                foreach (var record in csvData)
                {
                    csv.WriteField(record.TimeFrom);
                    csv.WriteField(record.TimeTo);

                    foreach (var unitName in allUnitNames)
                    {
                        if (record.Properties.ContainsKey(unitName))
                        {
                            csv.WriteField(record.Properties[unitName]);
                        }
                        else
                        {
                            csv.WriteField(string.Empty);
                        }
                    }
                    csv.NextRecord();
                }
            }

            Console.WriteLine("CSV file created successfully.");
        }   
    }
}