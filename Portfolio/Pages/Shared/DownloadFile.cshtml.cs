using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portfolio.Data;

namespace Portfolio.Pages.Shared
{
    [Authorize]
    public class DownloadFileModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public DownloadFileModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(int fileId)
        {
            var file = await _dbContext.Files.FindAsync(fileId);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.FileContent, file.ContentType, file.FileName);
        }
    }
}

