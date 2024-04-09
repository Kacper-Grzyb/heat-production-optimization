using heat_production_optimization.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;

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
            var stream = formFile.OpenReadStream();
            using (var reader = new StreamReader(stream))
            {
                Console.WriteLine(reader.ReadLine()); // skip the header files
                while (!reader.EndOfStream)
                {
                    string[] splitLine = reader.ReadLine().Split(";");
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
            return true;
        }
    }
}
