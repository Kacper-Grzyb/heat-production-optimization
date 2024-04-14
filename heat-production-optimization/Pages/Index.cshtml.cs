using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using heat_production_optimization.Pages.Shared;

namespace heat_production_optimization.Pages
{
    public class IndexModel : PageModel
    {
        public readonly SourceDataDbContext _context = new SourceDataDbContext();
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public IFormFile formFile { get; set; }

        public IndexModel(ILogger<IndexModel> logger, SourceDataDbContext context)
        {
            _logger = logger;
            if (context != null) _context = context;
        }

        public void OnGet()
        {

        }

        public void OnPost(string buttonAction, IFormFile uploadedFile)
        {
            IDataBaseManager sdm = new SourceDataManager(_context);
            switch (buttonAction)
            {
                case "uploadData":
                    if (uploadedFile == null || uploadedFile.ContentType != "text/csv" || uploadedFile.Length == 0)
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
                    if (sdm.LoadDbWithDanfossData(false))
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
