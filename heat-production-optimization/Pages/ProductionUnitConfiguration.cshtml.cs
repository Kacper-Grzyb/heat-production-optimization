using Google.OrTools.ConstraintSolver;
using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Text.RegularExpressions;

namespace heat_production_optimization.Pages
{
    public class PUCButtonRequest
    {
        public string action { get; set; } = string.Empty;
        public Guid unitId { get; set; }
    }

    public class CustomProductionUnitsModel : PageModel
    {
        readonly SourceDataDbContext _context;
        public DbSet<ProductionUnitDataModel> productionUnits { get; set; }

        [BindProperty]
        public PUCButtonRequest buttonAction { get; set; }

        [BindProperty]
        public ProductionUnitDataModel formProductionUnit { get; set; }

        public CustomProductionUnitsModel(SourceDataDbContext context)
        {
            _context = context;
            productionUnits = context.productionUnits;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(PUCButtonRequest request)
        {
            if (request == null || formProductionUnit == null) throw new Exception("The post request lacked arguments!");



			switch (request.action)
            {
                case "update":
					CustomUnit updatedUnit = new CustomUnit(
	                    Guid.NewGuid(),
	                    formProductionUnit.Alias,
	                    formProductionUnit.Name,
	                    formProductionUnit.MaxHeat,
	                    formProductionUnit.MaxElectricity,
	                    formProductionUnit.ProductionCostMWh,
	                    formProductionUnit.CO2EmissionMWh,
	                    formProductionUnit.GasConsumption,
	                    formProductionUnit.OilConsumption
                    );

					var unitToUpdate = _context.productionUnits.Find(request.unitId);
                    if (unitToUpdate == null) 
                    {
                        return NotFound();
                    }

                    unitToUpdate.Alias = updatedUnit.Alias;
                    unitToUpdate.Name = updatedUnit.Name;
                    unitToUpdate.MaxHeat = updatedUnit.MaxHeat;
                    unitToUpdate.MaxElectricity = updatedUnit.MaxElectricity;
                    unitToUpdate.ProductionCost = updatedUnit.ProductionCost;
                    unitToUpdate.ProductionCostMWh = updatedUnit.ProductionCostMWh;
                    unitToUpdate.CO2Emission = updatedUnit.CO2Emission;
                    unitToUpdate.CO2EmissionMWh = updatedUnit.CO2EmissionMWh;
                    unitToUpdate.GasConsumption = updatedUnit.GasConsumption;
                    unitToUpdate.OilConsumption = updatedUnit.OilConsumption;
                    unitToUpdate.PriceToHeatRatio = updatedUnit.PriceToHeatRatio;

                    _context.SaveChanges();
					break;
                case "create":
                    var (newUnitName, newUnitAlias) = GetNewUnitName(_context.productionUnits);
                    CustomUnit customUnit = new CustomUnit(Guid.NewGuid(), newUnitAlias, newUnitName);
                    _context.productionUnits.Add(customUnit);
                    _context.SaveChanges();
					break;
                case "delete":
                    var unitToDelete = _context.productionUnits.Find(request.unitId);
                    if(unitToDelete == null) 
                    { 
                        return NotFound(); 
                    }
                    _context.productionUnits.Remove(unitToDelete);
                    _context.SaveChanges();
                    break; 
                default:
                    break;
            }

			return RedirectToPage();
        }

        private (string, string) GetNewUnitName(DbSet<ProductionUnitDataModel> units)
        {
            int highestDigit = 1;
            foreach(var unit in units)
            {
				string regTemplate = @"^CustomBoiler[0-9]*$";
				if (Regex.Match(unit.Name, regTemplate).Success)
				{
                    if(!int.TryParse(unit.Name.Substring(12), out highestDigit)) 
                    { 
                        highestDigit = 1; 
                    }
                    else
                    {
                        highestDigit++;
                    }
				}
			}

            return ($"CustomBoiler{highestDigit}", $"CB{highestDigit}");

        }
    }
}
