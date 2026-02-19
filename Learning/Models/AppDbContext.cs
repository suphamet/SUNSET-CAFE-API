using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Learning.Models
{
    public class AppDbContext : DbContext
    {
        // บอกว่าเราจะใช้คลาส Product เชื่อมกับตารางใน Database
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // ตรงนี้สำคัญมาก: ให้คุณก๊อปปี้ Path ไฟล์ .db จาก DBeaver มาใส่แทนที่ตรงนี้
            // ตัวอย่าง: @"Data Source=D:\Test-DB\MyDataBase.db"
            options.UseSqlite(@"Data Source=D:\Test-DB\MyDataBase.db");
        }
    }
}