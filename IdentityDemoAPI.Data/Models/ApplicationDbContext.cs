using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data.Models
{
    public class ApplicationDbContext : IdentityDbContext
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
        }
        //End
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductMapCategory> ProductMapCategories { get; set; }
    }
}
