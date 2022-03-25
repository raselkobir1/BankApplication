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

            modelBuilder.Entity<BankAccount>()
                .HasOne(p => p.ApplicationUser)
                .WithMany()
                .HasForeignKey(m=> m.ApplicationUserId);

            modelBuilder.Entity<Balance>()
                .HasOne(b => b.BankAccount)
                .WithMany()
                .HasForeignKey(b => b.BankAccountId);
        }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<Balance>  Balances { get; set; }  
    }
}
