using backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>().ToTable("Accounts");
            builder.Entity<AccountGroup>().ToTable("AccountGroups");
            builder.Entity<Budget>().ToTable("Budgets");
            builder.Entity<Category>().ToTable("Categories");
            builder.Entity<Expense>().ToTable("Expenses");
            builder.Entity<Income>().ToTable("Incomes");
            builder.Entity<Notification>().ToTable("Notifications");

            builder.Entity<ApplicationUser>(e =>
            {
                e.ToTable("Users");
            });

            builder.Entity<IdentityUserClaim<string>>(e =>
            {
                e.ToTable("UserClaim");
            });

            builder.Entity<IdentityUserLogin<string>>(e =>
            {
                e.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<string>>(e =>
            {
                e.ToTable("UserTokens");
            });

            builder.Entity<IdentityRole>(e =>
            {
                e.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<string>>(e =>
            {
                e.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<string>>(e =>
            {
                e.ToTable("UserRoles")
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            });

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Account)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<Account>(a => a.UserId);

            builder.Entity<AccountGroup>()
                .HasMany(g => g.Accounts)
                .WithOne(a => a.AccountGroup)
                .HasForeignKey(a => a.AccountGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AccountGroup>()
                .HasOne(g => g.AdminUser)
                .WithMany()
                .HasForeignKey(g => g.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Expense>()
                .HasOne(e => e.AccountGroup)
                .WithMany(g => g.Expenses)
                .HasForeignKey(e => e.AccountGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Income>()
                .HasOne(i => i.AccountGroup)
                .WithMany()
                .HasForeignKey(i => i.AccountGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Budget>()
                .HasOne(b => b.AccountGroup)
                .WithMany()
                .HasForeignKey(b => b.AccountGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Category>()
                .HasOne(c => c.AccountGroup)
                .WithMany()
                .HasForeignKey(c => c.AccountGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Notification>()
                .HasOne(n => n.ApplicationUser)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Income>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);
        }
    }
}
