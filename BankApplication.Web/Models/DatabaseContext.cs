using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Web.Models
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public DatabaseContext()
        {
        }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
           .HasOne(p => p.ApplicationUser)
           .WithMany()
           .HasForeignKey(m=> m.ApplicationUserId);
        }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<Account>  Accounts { get; set; }
    }
}
