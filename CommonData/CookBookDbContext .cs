using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CommonData
{
    public class CookBookDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<DishProduct> DishProducts { get; set; }
        public DbSet<DishTag> DishTags { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Подключение к LocalDB с указанием пути к .mdf
        //    optionsBuilder.UseSqlServer
        //    (
        //        @"Server=(localdb)\mssqllocaldb;
        //      AttachDbFilename=C:\Users\Никита\source\repos\CookBook\CommonData\LocalDB\CookBookDB.mdf;
        //      Trusted_Connection=True;");
        //}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;AttachDbFilename=C:\Users\Никита\source\repos\CookBook\CommonData\LocalDB\CookBookDB.mdf; Database=CookBookDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи многие-ко-многим для DishProduct
            modelBuilder.Entity<DishProduct>()
                .HasKey(dp => new { dp.DishId, dp.ProductId });

            modelBuilder.Entity<DishProduct>()
                .HasOne(dp => dp.Dish)
                .WithMany(d => d.DishProducts)
                .HasForeignKey(dp => dp.DishId);

            modelBuilder.Entity<DishProduct>()
                .HasOne(dp => dp.Product)
                .WithMany(p => p.DishProducts)
                .HasForeignKey(dp => dp.ProductId);

            // Настройка связи многие-ко-многим для DishTag
            modelBuilder.Entity<DishTag>()
                .HasKey(dt => new { dt.DishId, dt.TagId });

            modelBuilder.Entity<DishTag>()
                .HasOne(dt => dt.Dish)
                .WithMany(d => d.DishTags)
                .HasForeignKey(dt => dt.DishId);

            modelBuilder.Entity<DishTag>()
                .HasOne(dt => dt.Tag)
                .WithMany(t => t.DishTags)
                .HasForeignKey(dt => dt.TagId);
        }
    }
}
