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

        public void OnPost(IFormFile uploadedFile)
        {
            if (uploadedFile.ContentType != "text/csv" || uploadedFile == null || uploadedFile.Length == 0)
            {
                Console.WriteLine("Wrong file format uploaded!");
                // show some message for the user
                return;
            }

            SourceDataManager sdm = new SourceDataManager(_context);

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

        }
    }
}
