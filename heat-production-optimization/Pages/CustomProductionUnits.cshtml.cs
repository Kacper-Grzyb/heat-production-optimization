using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Pages
{
    public class CustomProductionUnitsModel : PageModel
    {
        readonly SourceDataDbContext _context;
        public DbSet<ProductionUnitDataModel> productionUnits { get; set; }

        [BindProperty]
        public ProductionUnitDataModel newProductionUnit { get; set; }

        public CustomProductionUnitsModel(SourceDataDbContext context)
        {
            _context = context;
            productionUnits = context.productionUnits;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            //productionUnits.Add(new GasBoiler(Guid.NewGuid(), 5, 500, 215, 1.1, "GB", "Gas Boiler"));
            CustomUnit customUnit = new CustomUnit(
                Guid.NewGuid(),
                newProductionUnit.Alias,
                newProductionUnit.Name,
                newProductionUnit.MaxHeat,
                newProductionUnit.MaxElectricity,
                newProductionUnit.ProductionCostMWh,
                newProductionUnit.CO2EmissionMWh,
                newProductionUnit.GasConsumption,
                newProductionUnit.OilConsumption
            );
            _context.productionUnits.Add(customUnit);
            _context.SaveChanges();
			return RedirectToPage();
        }
    }
}
