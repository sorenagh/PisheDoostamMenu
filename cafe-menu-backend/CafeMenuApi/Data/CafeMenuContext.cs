using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Models;

namespace CafeMenuApi.Data
{
    public class CafeMenuContext : DbContext
    {
        public CafeMenuContext(DbContextOptions<CafeMenuContext> options) : base(options)
        {
        }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Admin> Admins { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Icon).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                
                // Configure relationship
                entity.HasMany(e => e.MenuItems)
                      .WithOne(e => e.Category)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configure MenuItem
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Image).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Photos);
            });
            
            // Configure Admin
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
            });
            
            // Seed initial data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Password = "admin123", // In production, this should be hashed
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
            
            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "قهوه", Icon = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=100&h=100&fit=crop", Description = "انواع قهوه‌های تازه و خوشمزه" },
                new Category { Id = 2, Name = "چای", Icon = "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=100&h=100&fit=crop", Description = "چای‌های معطر و گیاهی" },
                new Category { Id = 3, Name = "نوشیدنی سرد", Icon = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=100&h=100&fit=crop", Description = "نوشیدنی‌های خنک و طراوت‌بخش" },
                new Category { Id = 4, Name = "دسر", Icon = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=100&h=100&fit=crop", Description = "دسرهای خوشمزه و شیرین" },
                new Category { Id = 5, Name = "غذا", Icon = "https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=100&h=100&fit=crop", Description = "غذاهای اصلی و خوشمزه" },
                new Category { Id = 6, Name = "صبحانه", Icon = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=100&h=100&fit=crop", Description = "صبحانه‌های مقوی و سالم" }
            );
            
            // Seed menu items
            modelBuilder.Entity<MenuItem>().HasData(
                // Coffee items
                new MenuItem { Id = 1, Name = "اسپرسو", Price = 25000, CategoryId = 1, Image = "https://images.unsplash.com/photo-1510707577719-ae7c14805e3a?w=300&h=200&fit=crop", Description = "قهوه تلخ و قوی", Photos = "https://images.unsplash.com/photo-1510707577719-ae7c14805e3a?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1447933601403-0c6688de566e?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1542181961-9590d0c79dab?w=300&h=200&fit=crop" },
                new MenuItem { Id = 2, Name = "کاپوچینو", Price = 35000, CategoryId = 1, Image = "https://images.unsplash.com/photo-1534778101976-62847782c213?w=300&h=200&fit=crop", Description = "قهوه با شیر و فوم", Photos = "https://images.unsplash.com/photo-1534778101976-62847782c213?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=300&h=200&fit=crop" },
                
                // Tea items
                new MenuItem { Id = 3, Name = "چای سیاه", Price = 20000, CategoryId = 2, Image = "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=300&h=200&fit=crop", Description = "چای سنتی ایرانی", Photos = "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1576092768241-dec231879fc3?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1597318181409-cf85ba05cd49?w=300&h=200&fit=crop" },
                
                // Cold drinks
                new MenuItem { Id = 4, Name = "آیس کافی", Price = 40000, CategoryId = 3, Image = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=300&h=200&fit=crop", Description = "قهوه سرد و خوشمزه", Photos = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1497515114629-f71d768fd07c?w=300&h=200&fit=crop" },
                
                // Desserts
                new MenuItem { Id = 5, Name = "تیرامیسو", Price = 55000, CategoryId = 4, Image = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=300&h=200&fit=crop", Description = "دسر ایتالیایی خوشمزه", Photos = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300&h=200&fit=crop,https://images.unsplash.com/photo-1464349095431-e9a21285b5f3?w=300&h=200&fit=crop" }
            );
        }
    }
} 