using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portfolio.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Portfolio.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;
using Portfolio.Data.Models;
using System.Security.Claims;
using Portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Pages
{
    [Authorize]
    public class ActionFilesModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public ActionFilesModel(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public CalendarModel Calendar { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IList<MyAction> ActionsList { get; set; }
        public async Task<IActionResult> OnGetAsync(int? year, int? month)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("UserId is required.");
            }
            Year = year ?? DateTime.Now.Year;
            Month = month ?? DateTime.Now.Month;

            Calendar = new CalendarModel(Year, Month);

            ActionsList = await _dbContext.MyActions
                                      .Where(n => n.UserId == userId)
                                      .ToListAsync();
            return Page();
        }
    }

}