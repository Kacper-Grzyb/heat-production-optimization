using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using heat_production_optimization.Pages.Shared;
using static System.Net.Mime.MediaTypeNames;

namespace heat_production_optimization.Pages
{
    public class IndexModel : PageModel
    {
        public readonly SourceDataDbContext _context = new SourceDataDbContext();
        private readonly ILogger<IndexModel> _logger;

        public IUnit gasBoiler;
        public IUnit oilBoiler;
        public IUnit gasMotor;
        public IUnit electricBoiler;

		[BindProperty]
        public IFormFile formFile { get; set; }

        public IndexModel(ILogger<IndexModel> logger, SourceDataDbContext context)
        {
            _logger = logger;
            _context = context;
            gasBoiler = context.gasBoiler;
            oilBoiler = context.oilBoiler;
            gasMotor = context.gasMotor;
            electricBoiler = context.electricBoiler;
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
                    if (uploadedFile == null || (uploadedFile.ContentType != "text/csv" && uploadedFile.ContentType != "application/vnd.ms-excel" && uploadedFile.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") || uploadedFile.Length == 0)
                    {
                        _context.errorMessage = "Wrong file format uploaded!";
						_context.SaveChanges();
                        break;
                    }

                    if (!sdm.LoadDbWithInputData(uploadedFile))
                    { 
                        _context.errorMessage = "Failed to load data from upload!";
						_context.SaveChanges();
					}
                    break;
                case "loadDataSummer":
                    if (!sdm.LoadDbWithDanfossData(true))
                    {
                        _context.errorMessage = "Failed to load data!";
						_context.SaveChanges();
					}
                    break;
                case "loadDataWinter":
                    if (!sdm.LoadDbWithDanfossData(false))
                    {
                        _context.errorMessage = "Failed to load data!";
						_context.SaveChanges();
					}
                    break;
                default:
                    _context.errorMessage = "Wrong arguments provided to the function!";
                    break;
            }

        }
    }
}
