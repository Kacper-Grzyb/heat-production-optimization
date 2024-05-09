using heat_production_optimization;
using CsvHelper;
using System.Globalization;



namespace heat_production_optimization
{
    public class HourlyOptimization
    {
        public DateTime timeFrom { get; set; }
        public DateTime timeTo { get; set; }
        public static List<HourlyOptimization> HourlyOptimizations{ get; set; } = new List<HourlyOptimization> ();
        public double GasBolier { get; set; }
        public double OilBoiler { get; set; }
        public double GasMotor { get; set; }
        public double ElectricBoiler { get; set; }


    }
    public class SaveToCSV
    {
        public static int Nr {get; set; } = 0;
        public static void SaveOptimization()
        {
            if (HourlyOptimization.HourlyOptimizations == null || !HourlyOptimization.HourlyOptimizations.Any())
            {
                Console.WriteLine("No data available to write to CSV.");
                return;
            }
            string fileName = $"{Nr}_optimization";
            Nr++;
            using (var writer = new StreamWriter($"{fileName}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
            {
                csv.WriteRecords(HourlyOptimization.HourlyOptimizations);
            }

            Console.WriteLine("CSV file created successfully.");
        }
    }
}