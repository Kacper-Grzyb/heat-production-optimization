using heat_production_optimization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using heat_production_optimization.Pages.Shared;

namespace heat_production_optimization.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SourceDataDbContext _context = new SourceDataDbContext();
        private readonly ILogger<IndexModel> _logger;
        public _DataUploadModel _dataUploadModel;

        [BindProperty]
        public IFormFile formFile { get; set; }

        public IndexModel(ILogger<IndexModel> logger, SourceDataDbContext context)
        {
            _logger = logger;
            if (context != null) _context = context;
            _dataUploadModel = new _DataUploadModel(_context);
        }

        public void OnGet()
        {

        }
    }
}
