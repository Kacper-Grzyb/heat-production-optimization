using heat_production_optimization.Models;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace heat_production_optimization
{
    public class ExcelReader
    {
        public List<HeatDemandDataModel> Read(IFormFile upload, SourceDataDbContext context)
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
                    try
                    {
                        worksheet = package.Workbook.Worksheets[0];
                        rows = worksheet.Dimension.Rows;

                        if (worksheet.Dimension.Columns != 4)
                        {
                            context.errorMessage = "Wrong Excel Worksheet format!";
                            return new List<HeatDemandDataModel>();
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

                            HeatDemandDataModel dataRow = new HeatDemandDataModel() { Id = index++, timeFrom = timeFrom, timeTo = timeTo, heatDemand = Math.Round(heatDemand, 2), electricityPrice = Math.Round(electricityPrice, 2) };

                            readResult.Add(dataRow);
                        }
                    }
                    catch
                    {
                        context.errorMessage = "Wrong Excel Worksheet format!";
                        return new List<HeatDemandDataModel>();
                    }

				}
            }

            return readResult;
        }
}

    public class ExcelWriter
    {
        public byte[] Write(List<UnitUsageDataModel> unitUsages, OptimizerResultsDataModel results, List<ProductionUnitDataModel> productionUnits)
        {
            ExcelPackage package = new ExcelPackage();
            ExcelWorkbook workbook = package.Workbook;
            ExcelWorksheet worksheet = workbook.Worksheets.Add("Optimized Results");

            int rowIndex = 1;
            int columnIndex = 1;

            try
            {
                // Adding column headers
                worksheet.Cells[rowIndex, columnIndex++].Value = "Time from";
                worksheet.Cells[rowIndex, columnIndex++].Value = "Time to";
                foreach(var unit in productionUnits)
                {
                    worksheet.Cells[rowIndex, columnIndex++].Value = unit.Name;
                }

                rowIndex = 2;
                columnIndex = 1;

                // Adding production unit usage data
                foreach(UnitUsageDataModel item in unitUsages)
                {
                    worksheet.Cells[rowIndex, columnIndex++].Value = item.DateInterval.TimeFrom;
                    worksheet.Cells[rowIndex, columnIndex++].Value = item.DateInterval.TimeTo;
                    foreach(var unit in productionUnits)
                    {
                        worksheet.Cells[rowIndex, columnIndex++].Value = item.activationsDictionary[unit];
                    }
                    rowIndex++;
                    columnIndex = 1;
                }

                // Adding optimization results data
                rowIndex++;
                columnIndex = 1;

                worksheet.Cells[rowIndex, columnIndex++].Value = "Total heat production";
				worksheet.Cells[rowIndex, columnIndex++].Value = "Total electricity production";
				worksheet.Cells[rowIndex, columnIndex++].Value = "Expenses";
				worksheet.Cells[rowIndex, columnIndex++].Value = "Consumption of gas";
				worksheet.Cells[rowIndex, columnIndex++].Value = "Consumption of oil";
				worksheet.Cells[rowIndex, columnIndex++].Value = "Consumption of electricity";
				worksheet.Cells[rowIndex, columnIndex++].Value = "Produced CO2";

				rowIndex++;
				columnIndex = 1;

                worksheet.Cells[rowIndex, columnIndex++].Value = results.TotalHeatProduction;
                worksheet.Cells[rowIndex, columnIndex++].Value = results.TotalElectricityProduction;
                worksheet.Cells[rowIndex, columnIndex++].Value = results.Expenses;
                worksheet.Cells[rowIndex, columnIndex++].Value = results.ConsumptionOfGas;
                worksheet.Cells[rowIndex, columnIndex++].Value = results.ConsumptionOfOil;
                worksheet.Cells[rowIndex, columnIndex++].Value = results.ConsumptionOfElectricity;
                worksheet.Cells[rowIndex, columnIndex++].Value = results.ProducedCO2;


				using (MemoryStream stream  = new MemoryStream())
                {
                    package.SaveAs(stream);
                    byte[] content = stream.ToArray();
                    return content;
				}
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
