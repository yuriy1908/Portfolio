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
    public class UploadFilesModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AplicationIdentityUser> _userManager;

        public UploadFilesModel(ApplicationDbContext dbContext, UserManager<AplicationIdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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

        [BindProperty]
        public string Source { get; set; }
        [BindProperty]
        public string Time { get; set; }
        [BindProperty]
        public string Date { get; set; }
        [BindProperty]
        public string Text { get; set; }
        [BindProperty]
        public IFormFile FileUploadControl { get; set; }
        public string StatusMessage;
        public async Task<IActionResult> OnPost()
        {
            if (Source == "modalForm")
            {
                var submittedDate = Date;
                var submittedTime = Time;
                var submittedText = Text;
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    await UploadNote(Date, Time, Text, userId);

                    StatusMessage = "Successfully";
                }
                catch (Exception ex)
                {
                    StatusMessage = ex.Message;
                };
                
            }
            else
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

                
            }
            await OnGetAsync();
            return Page();
        }
        public async Task UploadNote(string date, string time, string text, string userId)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(text))
            {
                throw new Exception("At least one of the parameters (date, time, text) must be provided.");
            }

            var note = new Note
            {
                Text = text,
                UserId = userId,
                Date = date,
                Time = time
            };

            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();
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