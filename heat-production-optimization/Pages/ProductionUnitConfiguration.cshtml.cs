﻿using Google.OrTools.ConstraintSolver;
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

    public class ProductionUnitConfigurationModel : PageModel
    {
        readonly SourceDataDbContext _context;
        public DbSet<ProductionUnitDataModel> productionUnits { get; set; }
        public string? errorMessage { get; set; }

        [BindProperty]
        public PUCButtonRequest buttonAction { get; set; }

        [BindProperty]
        public ProductionUnitDataModel formProductionUnit { get; set; }

        public ProductionUnitConfigurationModel(SourceDataDbContext context)
        {
            _context = context;
            productionUnits = context.productionUnits;
            errorMessage = context.uiMessages.Find(MessageType.AddUnitError)?.Message;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(PUCButtonRequest request)
        {
            Console.WriteLine();
            if (request == null) throw new Exception("The post request lacked arguments!");
            Console.WriteLine();
			switch (request.action)
            {
                case "update":
                    return UpdateUnit(request);
                case "create":
                    return CreateUnit();
                case "delete":
                    return DeleteUnit(request);
                default:
                    break;
            }

			return RedirectToPage();
        }

        private IActionResult UpdateUnit(PUCButtonRequest request)
        {
			if (formProductionUnit == null)
			{
				throw new Exception("The post request did not give any data to update the unit with!");
			}

            IUnit updatedUnit;

            if(_context.productionUnits.Find(request.unitId)?.Alias == "GB")
            {
                updatedUnit = new GasBoiler(
                    request.unitId,
                    formProductionUnit.MaxHeat,
                    formProductionUnit.ProductionCostMWh,
                    formProductionUnit.CO2EmissionMWh,
                    formProductionUnit.GasConsumption
                );
            }
            else if(_context.productionUnits.Find(request.unitId)?.Alias == "OB")
            {
                updatedUnit = new OilBoiler(
                    request.unitId,
                    formProductionUnit.MaxHeat,
                    formProductionUnit.ProductionCostMWh,
                    formProductionUnit.CO2Emission,
                    formProductionUnit.OilConsumption
                );
            }
            else if(_context.productionUnits.Find(request.unitId)?.Alias == "GM")
            {
                updatedUnit = new GasMotor(
                    request.unitId,
                    formProductionUnit.MaxHeat,
                    formProductionUnit.MaxElectricity,
                    formProductionUnit.ProductionCostMWh,
                    formProductionUnit.CO2EmissionMWh,
                    formProductionUnit.GasConsumption
                );
            }
            else if(_context.productionUnits.Find(request.unitId)?.Alias == "EK")
            {
                updatedUnit = new ElectricBoiler(
                    request.unitId,
                    formProductionUnit.MaxHeat,
                    formProductionUnit.MaxElectricity,
                    formProductionUnit.ProductionCostMWh,
                    formProductionUnit.CO2EmissionMWh
                );
            }
            else
            {
                switch(formProductionUnit.Alias)
                {
                    case "GB":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's alias to GB!";
						_context.SaveChanges();
						return RedirectToPage();
                    case "OB":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's alias to OB!";
						_context.SaveChanges();
						return RedirectToPage();
                    case "GM":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's alias to GM!";
						_context.SaveChanges();
						return RedirectToPage();
                    case "EK":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's alias to EK!";
						_context.SaveChanges();
						return RedirectToPage();
                    default:
                        break;
				}

                switch(formProductionUnit.Name)
                {
                    case "Gas Boiler":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a customs boiler's name to Gas Boiler!";
						_context.SaveChanges();
						return RedirectToPage();
                    case "Oil Boiler":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's name to Oil Boiler!";
						_context.SaveChanges();
						return RedirectToPage();
                    case "Gas Motor":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's name to Gas Motor!";
						_context.SaveChanges();
						return RedirectToPage();
                    case "Electric Boiler":
						_context.uiMessages.Find(MessageType.AddUnitError).Message = "❌ Cannot set a custom boiler's name to Electric Boiler!";
						_context.SaveChanges();
						return RedirectToPage();
                    default:
                        break;
				}

				updatedUnit = new CustomUnit(
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
			}

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

			_context.uiMessages.Find(MessageType.AddUnitError).Message = string.Empty;
			_context.SaveChanges();
            return RedirectToPage();
		}

        private IActionResult CreateUnit()
        {
			var (newUnitName, newUnitAlias) = GetNewUnitName(_context.productionUnits);
			CustomUnit customUnit = new CustomUnit(Guid.NewGuid(), newUnitAlias, newUnitName);
			_context.productionUnits.Add(customUnit);
			_context.uiMessages.Find(MessageType.AddUnitError).Message = string.Empty;
			_context.SaveChanges();
            return RedirectToPage();
		}

        private IActionResult DeleteUnit(PUCButtonRequest request)
        {
			var unitToDelete = _context.productionUnits.Find(request.unitId);
			if (unitToDelete == null)
			{
				return NotFound();
			}
			_context.productionUnits.Remove(unitToDelete);
			_context.uiMessages.Find(MessageType.AddUnitError).Message = string.Empty;
			_context.SaveChanges();
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
