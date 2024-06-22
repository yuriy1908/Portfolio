using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Identity;
using Portfolio.Data.Models;

namespace Portfolio.Data
{
    public class ApplicationDbContext : IdentityDbContext<AplicationIdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<UploadedFile> Files { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<MyAction> MyActions { get; set; }
    }
}
