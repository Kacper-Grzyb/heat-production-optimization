using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Office.Interop.Excel;
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

            if (formFile.ContentType == "text/csv" || formFile.ContentType == "application/vnd.ms-excel")
            {
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
                            temp.Id = id++;
                            _context.HeatDemandData.Add(temp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There was an error during loading the file! Exception message {ex.Message}");
                    return false;
                }

                _context.loadedDataPath = formFile.FileName;
                _context.SaveChanges();
                _context.errorMessage = string.Empty;
                _context.SaveChanges();
                return true;
            }

			else if(formFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                ExcelReader reader = new ExcelReader();
                List<HeatDemandDataModel> data = reader.Read(formFile);
                foreach(HeatDemandDataModel record in data) 
                {
                    _context.HeatDemandData.Add(record);    
                }

                _context.loadedDataPath = formFile.FileName;
                _context.SaveChanges();
                _context.errorMessage = string.Empty;
                _context.SaveChanges();
                return true;
            }

            return false;
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


			_context.loadedDataPath = dataPath;
			_context.errorMessage = string.Empty;
			_context.SaveChanges();
			return true;
        }

        public void ClearDatabase()
        {
			if (_context.IsDataLoaded())
            {
                _context.HeatDemandData.RemoveRange(_context.HeatDemandData);
                _context.loadedDataPath = "";
                _context.SaveChanges();
			}
		}
    }

    public class DoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			if (valueProviderResult != ValueProviderResult.None)
			{
				var value = valueProviderResult.FirstValue;

				if (!string.IsNullOrEmpty(value))
				{
					if (value.Contains('.') || value.Contains(','))
                    {
						// Error handling
						int dots = value.Count(x => x == '.');
                        int commas = value.Count(x => x == ',');
                        
                        if(dots > 1 || commas > 1 || (dots>0 && commas>0))
                        {
							bindingContext.Result = ModelBindingResult.Failed();
							return Task.CompletedTask;
						}
                        else
                        {
							double decimalResult = GetDecimal(value);
							bindingContext.Result = ModelBindingResult.Success(decimalResult);
						}

                        return Task.CompletedTask;
                    }

					// Else if the number does not have decimal values
					if (double.TryParse(value, NumberStyles.Any, new CultureInfo("de-DE"), out double result))
					{
						bindingContext.Result = ModelBindingResult.Success(result);
						return Task.CompletedTask;
					}
				}

				bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid double value.");
			}

			bindingContext.Result = ModelBindingResult.Failed();
			return Task.CompletedTask;
		}

        private double GetDecimal(string numberText)
        {
            if(numberText.Contains(","))
            {
                numberText = numberText.Replace(',', '.');
            }
			return double.Parse(numberText, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
    }

	public class DoubleModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (context.Metadata.ModelType == typeof(double) || context.Metadata.ModelType == typeof(double?))
			{
				return new BinderTypeModelBinder(typeof(DoubleModelBinder));
			}

			return null;
		}
	}
}
