using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;

namespace heat_production_optimization.ViewComponents
{
	public class DataUploadViewComponent:ViewComponent
	{
		private readonly SourceDataDbContext _context;

        public DataUploadViewComponent(SourceDataDbContext context)
        {
			_context = context;
        }

        public IViewComponentResult Invoke()
		{
			return View(_context);
		}

	}
}
