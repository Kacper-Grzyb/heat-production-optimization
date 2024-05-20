using System.Net.Sockets;
using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

            var csvBuilder = new StringBuilder();
            foreach (var item in HourlyOptimization.HourlyOptimizations)
            {
                csvBuilder.AppendLine($"{item.timeFrom},{item.timeTo},{item.properties}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(buffer, "text/csv", "optimized_results.csv");
        }  

    }
}

