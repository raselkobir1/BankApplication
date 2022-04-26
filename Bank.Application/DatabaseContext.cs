using Bank.Entity.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application
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
                .HasForeignKey(m => m.ApplicationUserId);

            modelBuilder.Entity<Balance>()
                .HasOne(b => b.BankAccount)
                .WithMany()
                .HasForeignKey(b => b.BankAccountId);

            modelBuilder.Entity<UserInvitation>()
                .HasOne(i => i.InvitedBy)
                .WithMany()
                .HasForeignKey(i => i.InvitedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserInvitation>()
                .HasOne(i => i.AcceptedBy)
                .WithMany()
                .HasForeignKey(i => i.AcceptedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PromoCode>()
                .HasKey(p => p.Id);
        }

        //public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<UserInvitation> UserInvitations { get; set; }   
    }
}
