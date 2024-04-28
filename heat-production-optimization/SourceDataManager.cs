using heat_production_optimization.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335; 
using System.Globalization;

namespace heat_production_optimization
{
    public class SourceDataManager : IDataBaseManager
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();

        public SourceDataManager(SourceDataDbContext context) 
        {
            if (context != null) _context = context;
        }

        public bool LoadDbWithInputData(IFormFile formFile)
        {
            if (_context.IsDataLoaded()) ClearDatabase();

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
                        temp.timeFrom = DateTime.ParseExact(splitLine[0], "dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("en-GB"));
                        temp.timeTo = DateTime.ParseExact(splitLine[1], "dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("en-GB"));
                        temp.heatDemand = double.Parse(splitLine[2], CultureInfo.GetCultureInfo("da-DK"));
                        temp.electricityPrice = double.Parse(splitLine[3], CultureInfo.GetCultureInfo("da-DK"));
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

			_context.loadedDataPath = formFile.FileName;
			_context.errorMessage = string.Empty;
			_context.SaveChanges();
			return true;
        }

        public bool LoadDbWithDanfossData(bool summerPeriod)
        {
			if (_context.IsDataLoaded()) ClearDatabase();

			int id = 1;
            string dataPath;
            if (summerPeriod) dataPath = "DanfossData_Summer.csv";
            else dataPath = "DanfossData_Winter.csv";

            try
            {
                using (StreamReader reader = new StreamReader("wwwroot/danfoss_data/" + dataPath))
                {
                    reader.ReadLine(); // skip the header files

                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine();
                        if (line == null) continue;

                        string[] splitLine = line.Split(";");
                        HeatDemandDataModel temp = new HeatDemandDataModel();
                        temp.timeFrom = DateTime.Parse(splitLine[0], CultureInfo.GetCultureInfo("en-GB"));
                        temp.timeTo = DateTime.Parse(splitLine[1], CultureInfo.GetCultureInfo("en-GB"));
                        temp.heatDemand = double.Parse(splitLine[2], CultureInfo.GetCultureInfo("da-DK"));
                        temp.electricityPrice = double.Parse(splitLine[3], CultureInfo.GetCultureInfo("da-DK"));
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


			_context.loadedDataPath = dataPath;
			_context.errorMessage = string.Empty;
			_context.SaveChanges();
			return true;
        }

        public void ClearDatabase()
        {
			if (_context.IsDataLoaded())
			{
				foreach (var item in _context.HeatDemandData)
				{
					_context.HeatDemandData.Remove(item);
				}

                _context.loadedDataPath = "";
                _context.SaveChanges();
			}
		}
    }
}
