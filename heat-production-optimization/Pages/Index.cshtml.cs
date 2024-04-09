using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace heat_production_optimization.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();
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

        public void OnPost(IFormFile uploadedFile)
        {
            if (uploadedFile.ContentType != "text/csv" || uploadedFile == null || uploadedFile.Length == 0)
            {
                Console.WriteLine("Wrong file format uploaded!");
                // show some message for the user
                return;
            }

            SourceDataManager sdm = new SourceDataManager(_context);
            
            if(sdm.LoadDbWithInputData(uploadedFile))
            {
                Console.WriteLine("Data loaded successfully!");
                // display a message for the user
            }
            else
            {
                Console.WriteLine("Failed to load data from upload!");
                // display a message for the user
            }

        }
    }
}
