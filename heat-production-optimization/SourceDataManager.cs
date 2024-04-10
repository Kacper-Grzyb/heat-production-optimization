using heat_production_optimization.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace heat_production_optimization
{
    public class SourceDataManager
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();

        public SourceDataManager(SourceDataDbContext context) 
        {
            if (context != null) _context = context;
        }

        public bool LoadDbWithInputData(IFormFile formFile)
        {
            int id = 1;
            if (formFile == null) return false;

            try
            {
                var stream = formFile.OpenReadStream();
                using (var reader = new StreamReader(stream))
                {
                    reader.ReadLine(); // skip the header files
                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine();
                        if (line == null) continue;

                        string[] splitLine = line.Split(";");
                        HeatDemandDataModel temp = new HeatDemandDataModel();
                        temp.timeFrom = DateTime.Parse(splitLine[0]);
                        temp.timeTo = DateTime.Parse(splitLine[1]);
                        temp.heatDemand = double.Parse(splitLine[2]);
                        temp.electricityPrice = double.Parse(splitLine[3]);
                        temp.Id = id;
                        id++;
                        _context.HeatDemandData.Add(temp);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was an error during loading the file! Exception message {ex.Message}");
                return false;
            }

            return true;
        }

        public bool LoadDbWithDanfossData(bool summerPeriod)
        {
            int id = 1;
            string dataPath;
            if (summerPeriod) dataPath = "~/danfoss_data/DanfossData_Summer.csv";
            else dataPath = "~/danfoss_data/DanfossData_Winter.csv";

            try
            {
                using (StreamReader reader = new StreamReader(dataPath))
                {
                    reader.ReadLine(); // skip the header files

                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine();
                        if (line == null) continue;

                        string[] splitLine = line.Split(";");
                        HeatDemandDataModel temp = new HeatDemandDataModel();
                        temp.timeFrom = DateTime.Parse(splitLine[0]);
                        temp.timeTo = DateTime.Parse(splitLine[1]);
                        temp.heatDemand = double.Parse(splitLine[2]);
                        temp.electricityPrice = double.Parse(splitLine[3]);
                        temp.Id = id;
                        id++;
                        _context.HeatDemandData.Add(temp);
                    }
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"There was an error during loading the data! Exception message: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}
