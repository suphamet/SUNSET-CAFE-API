using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Learning.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderMenu> OrderMenus { get; set; } // ตรวจสอบว่า Class ชื่อ Order
        public DbSet<DetailOrder> DetailOrders { get; set; } // ตรวจสอบว่า Class ชื่อ DetailOrder
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source=D:\Test-DB\MyDataBase.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderMenu>().ToTable("OrderMenu");
            modelBuilder.Entity<DetailOrder>().ToTable("DetailOrder");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}