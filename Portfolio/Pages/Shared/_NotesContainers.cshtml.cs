using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Data.Identity;
using Portfolio.Data.Models;
using System.Security.Claims;

namespace Portfolio.Pages.Shared
{
    public class NotesContainersModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public NotesContainersModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Note> NotesList { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("UserId is required.");
            }

            NotesList = await _dbContext.Notes
                                      .Where(n => n.UserId == userId)
                                      .ToListAsync();

            return Page();
        }
    }
}
