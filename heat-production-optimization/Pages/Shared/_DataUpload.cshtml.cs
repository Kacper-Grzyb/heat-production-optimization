using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace heat_production_optimization.Pages.Shared
{
    public class _DataUploadModel : PageModel
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();

        public _DataUploadModel(SourceDataDbContext context)
        {
            if (context != null) _context = context;
        }

        public void OnGet()
        {
        }

        public void OnPost(string buttonAction, IFormFile uploadedFile)
        {
			SourceDataManager sdm = new SourceDataManager(_context);
			switch (buttonAction)
            {
                case "uploadData":
					if (uploadedFile.ContentType != "text/csv" || uploadedFile == null || uploadedFile.Length == 0)
					{
						Console.WriteLine("Wrong file format uploaded!");
						// show some message for the user
						return;
					}

					if (sdm.LoadDbWithInputData(uploadedFile))
					{
						Console.WriteLine("Data loaded successfully!");
						// display a message for the user
					}
					else
					{
						Console.WriteLine("Failed to load data from upload!");
						// display a message for the user
					}
					break;
				case "loadDataSummer":
					if (sdm.LoadDbWithDanfossData(true))
					{
						Console.WriteLine("Data loaded successfully!");
					}
					else
					{
						Console.WriteLine("Failed to load data!");
					}
					break;
				case "loadDataWinter":
					if(sdm.LoadDbWithDanfossData(false))
					{
						Console.WriteLine("Data loaded successfully!");
					}
					else
					{
						Console.WriteLine("Failed to load data!");
					}
					break;
				default:
					Console.WriteLine("Wrong arguments provided to the function!");
					break;
            }


        }
    }
}
