using heat_production_optimization.Models;
using OfficeOpenXml;

namespace heat_production_optimization
{
    public class ExcelReader
    {
        public List<HeatDemandDataModel> Read(IFormFile upload)
        {
            ExcelWorksheet worksheet;

            string str = string.Empty;
            int rows = 0;
            int index = 1;
            DateTime timeFrom;
            DateTime timeTo;
            double heatDemand = 0;
            double electricityPrice = 0;

            List<HeatDemandDataModel> readResult = new();

            using(var memoryStream = new MemoryStream())
            {
                upload.CopyTo(memoryStream);

                using(var package = new ExcelPackage(memoryStream))
                {
                    worksheet = package.Workbook.Worksheets[0];
                    rows = worksheet.Dimension.Rows;

                    if(worksheet.Dimension.Columns != 4)
                    {
                        throw new Exception("Wrong Excel Worksheet format!");
                    }

					for (int i = 1; i <= rows; i++)
					{
						if (!DateTime.TryParse(worksheet.Cells[i, 1].Value.ToString(), out timeFrom))
						{
							throw new Exception($"Cell at row {i} column 1 could not be parsed as a DateTime timeFrom value!");
						}
						if (!DateTime.TryParse(worksheet.Cells[i, 2].Value.ToString(), out timeTo))
						{
							throw new Exception($"Cell at row {i} column 2 could not be parsed as a DateTime timeTo value!");
						}
						if (!double.TryParse(worksheet.Cells[i, 3].Value.ToString(), out heatDemand))
						{
							throw new Exception($"Cell at row {i} column 3 could not be parsed as a DateTime heatDemand value!");
						}
						if (!double.TryParse(worksheet.Cells[i, 4].Value.ToString(), out electricityPrice))
						{
							throw new Exception($"Cell at row {i} column 4 could not be parsed as a DateTime electricityPrice value!");
						}

						HeatDemandDataModel dataRow = new HeatDemandDataModel() { Id = index++, timeFrom = timeFrom, timeTo = timeTo, heatDemand = Math.Round(heatDemand, 2), electricityPrice = Math.Round(electricityPrice,2) };

						readResult.Add(dataRow);
					}
				}
            }

            return readResult;
        }
}

    public class ExcelWriter
    {

    }
}
