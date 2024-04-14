using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;

namespace heat_production_optimization.ViewComponents
{
	public class DataUploadViewComponent:ViewComponent
	{

        public IViewComponentResult Invoke()
		{
			return View();
		}

	}
}
