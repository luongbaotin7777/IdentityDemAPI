
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,AppRole,Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        //Fluent API Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Product Configuration
            modelBuilder.Entity<Product>().ToTable("Products").HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("Decimal(10,2)").IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(100).IsRequired();
            //End
            //Category Configuration
            modelBuilder.Entity<Category>().ToTable("Categories").HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(100).IsRequired();
            //End
            //ProductMapCategory Configuration
            modelBuilder.Entity<ProductMapCategory>().ToTable("ProductMapCategories")
                .HasKey(pmc => new { pmc.ProductId, pmc.CategoryId });
            modelBuilder.Entity<ProductMapCategory>().HasOne(pmc => pmc.Product)
                .WithMany(p => p.ProductMapCategories).HasForeignKey(pmc => pmc.ProductId);
            modelBuilder.Entity<ProductMapCategory>().HasOne(pmc => pmc.Category)
                .WithMany(c => c.ProductMapCategories).HasForeignKey(pmc => pmc.CategoryId);
            //End
            /* Identity Class Configuration*/
            //AppUser Configuration
            modelBuilder.Entity<AppUser>().ToTable("AppUsers").HasKey(u => u.Id);
            modelBuilder.Entity<AppUser>().Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<AppUser>().Property(u => u.LastName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<AppUser>().Property(u => u.Dob).IsRequired();
            //AppRole Configuration
            modelBuilder.Entity<AppRole>().ToTable("AppRoles").HasKey(r => r.Id);
            modelBuilder.Entity<AppRole>().Property(r => r.Name).HasMaxLength(100).IsRequired();
            //Anonther class in Indentity...
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(uc => uc.UserId);
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(ur => new { ur.RoleId, ur.UserId });
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(ul => ul.UserId);
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppUserRoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(ut => ut.UserId);
            
        }
        //End
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductMapCategory> ProductMapCategories { get; set; }
    }
}
