using heat_production_optimization;
using CsvHelper;
using System.Globalization;



namespace heat_production_optimization
{
    public class HourlyOptimization
    {
        public Tuple<DateTime, DateTime>? TimeFrame { get; set; }
        public static List<HourlyOptimization>? HourlyOptimizations{ get; set; }
        public double GasBolier { get; set; }
        public double OilBoiler { get; set; }
        public double GasMotor { get; set; }
        public double ElectricBoiler { get; set; }


    }
    public class SaveToCSV
    {
        public int Nr {get; set; } = 0;
        public void SaveOptimization()
        {
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