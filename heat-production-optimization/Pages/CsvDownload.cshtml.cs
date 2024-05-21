using System.Net.Sockets;
using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using CsvHelper;
using System.Globalization;

namespace heat_production_optimization.Pages
{
    public class CsvDownloadModel : PageModel
    {
		private readonly SourceDataDbContext _context;
		private List<UnitUsageDataModel> unitUsageData;

		public CsvDownloadModel(SourceDataDbContext context)
		{
			_context = context;
			// black magic
			unitUsageData = context.unitUsage
				.Include(i => i.DateInterval)
				.Include(i => i.ActivationPercentages)
					.ThenInclude(a => a.Unit)
				.ToList();
		}

        public List<HourlyOptimization> hourlyOptimizations{ get; set; }=HourlyOptimization.HourlyOptimizations;
        public int numberOfHours = HourlyOptimization.HourlyOptimizations.Count;
        
        public IActionResult OnGet()
        {
            var csvWriter = new CSVWriter();

            string csvContent = csvWriter.Write(unitUsageData, _context.optimizerResults.First());

            byte[] byteArray = Encoding.UTF8.GetBytes(csvContent);

            return File(byteArray, "text/csv", "OptimizationData.csv");
        }

    }
}

