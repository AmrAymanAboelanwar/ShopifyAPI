using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class ShopifyContext: IdentityDbContext<ApplicationUser>
    {


        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem>  CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Governorate> Governorates { get; set; }
        public virtual DbSet<Payment> Payments  { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImages> ProductImages { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryProduct> InventoryProducts { get; set; }
        public virtual DbSet<Promotions> Promotions { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<View> Views { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Customer>().ToTable("Customers");
            builder.Entity<Seller>().ToTable("Sellers");
            builder.Entity<Employee>().ToTable("Employees");

           

            builder.Entity<View>().HasKey(v => new { v.CustomerId, v.ProductId });
            builder.Entity<Review>().HasKey(r => new { r.CustomerId, r.ProductId });
           // builder.Entity<ProductImages>().HasKey(pi => new { pi.ProductId});
            builder.Entity<ProductImages>().HasKey(pi => new { pi.ProductId, pi.Image });
            builder.Entity<InventoryProduct>().HasKey(IP => new { IP.InventoryId,IP.ProductId });


            builder.Entity<Category>()
           .Property(b => b.CreatedAt)
           .HasDefaultValueSql("getdate()");

            builder.Entity<SubCategory>()
          .Property(b => b.CreatedAt)
          .HasDefaultValueSql("getdate()");


            builder.Entity<Brand>()
          .Property(b => b.CreatedAt)
          .HasDefaultValueSql("getdate()");



         builder.Entity<Employee>()
        .Property(b => b.hireDate)
        .HasDefaultValueSql("getdate()");


        }

        public ShopifyContext(DbContextOptions options) : base(options)
        {
        }

   


    }
}
