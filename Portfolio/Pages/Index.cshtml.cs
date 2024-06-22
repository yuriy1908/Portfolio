using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Data.Models;
using System.Data.SqlTypes;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Portfolio.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public IList<Note> NotesList { get; set; }
        public IList<MyAction> ActionsList { get; set; }
        public string CurrentDateTime { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            string DayOfWeek = DateTime.Now.ToString("dddd");
            DayOfWeek = DayOfWeek[0].ToString().ToUpper() + DayOfWeek.Substring(1);
            CurrentDateTime = DayOfWeek +' '+ DateTime.Now.ToString("f");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("UserId is required.");
            }

            NotesList = await _dbContext.Notes
                                      .Where(n => n.UserId == userId)
                                      .ToListAsync();


            ActionsList = await _dbContext.MyActions
                                      .Where(n => n.UserId == userId)
                                      .ToListAsync();

            return Page();

        }
        [BindProperty]
        public string Source { get; set; }
        [BindProperty]
        public string Date { get; set; }
        [BindProperty]
        public string Name { get; set; }

        public string StatusMessage;
        public async Task<IActionResult> OnPost()
        {
            var submittedDate = Date;
            var submittedName = Name;
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                await UploadAction(submittedDate, submittedName, userId);

                StatusMessage = "Successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            };
            await OnGetAsync();
            return Page();
        }
        public async Task UploadAction(string date, string name, string userId)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(name))
            {
                throw new Exception("At least one of the parameters (date, time, text) must be provided.");
            }

            var myAction = new MyAction
            {
                Name = name,
                UserId = userId,
                Date = date,
            };

            _dbContext.MyActions.Add(myAction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
