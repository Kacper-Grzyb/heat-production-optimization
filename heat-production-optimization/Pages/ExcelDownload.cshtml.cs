using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Pages
{
    public class ExcelDownloadModel : PageModel
    {
		private readonly SourceDataDbContext _context;
		private List<UnitUsageDataModel> unitUsageData;

		public ExcelDownloadModel(SourceDataDbContext context)
		{
			_context = context;
			// black magic
			unitUsageData = context.unitUsage
				.Include(i => i.DateInterval)
				.Include(i => i.ActivationPercentages)
					.ThenInclude(a => a.Unit)
				.ToList();
		}

		public FileResult OnGet()
        {
			ExcelWriter writer = new ExcelWriter();

			var content = writer.Write(unitUsageData, _context.optimizerResults.First());
			string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			string fileName = "OptimizationData.xlsx";
			return File(content, contentType, fileName);
		}
    }
}
