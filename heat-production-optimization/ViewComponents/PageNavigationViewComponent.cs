using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;

namespace heat_production_optimization.ViewComponents
{
	public class PageNavigationViewComponent : ViewComponent
	{
		private readonly SourceDataDbContext _context;

        public PageNavigationViewComponent(SourceDataDbContext context)
        {
			_context = context;
        }

        public IViewComponentResult Invoke()
		{
			var context = _context;
			return View(context);
		}
	}
}
