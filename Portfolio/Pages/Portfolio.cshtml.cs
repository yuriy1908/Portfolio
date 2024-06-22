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
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Portfolio.Pages
{
    [Authorize]
    public class PortfolioModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AplicationIdentityUser> _userManager;

        public PortfolioModel(ApplicationDbContext dbContext, UserManager<AplicationIdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        [BindProperty]
        public IFormFile FileUploadControl { get; set; }
        public string StatusMessage;
        public List<UploadedFile> Files { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Files = await _dbContext.Files.Where(f => f.UserId == userId).ToListAsync();
        }
        public async Task<IActionResult> OnPost()
        {
            
            if (FileUploadControl != null && FileUploadControl.Length > 0)
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    await UploadFileToDatabase(FileUploadControl, userId);

                    StatusMessage = "Upload status: File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    StatusMessage = "Upload status: The file could not be uploaded. The following error occurred: " + ex.Message;
                }
            }
            else
            {
                StatusMessage = "Upload status: Please select a file to upload.";
            }
            await OnGetAsync();
            return Page();
        }
        public async Task UploadFileToDatabase(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty");
            }

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var newFile = new UploadedFile
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileContent = fileBytes,
                UserId = userId
            };

            _dbContext.Files.Add(newFile);
            await _dbContext.SaveChangesAsync();
        }
    }

}