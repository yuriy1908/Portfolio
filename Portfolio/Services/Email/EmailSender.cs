using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Identity;
using Portfolio.Data;

namespace Portfolio.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //TODO Create EmailSender
            await Task.CompletedTask;
        }
    }
}
