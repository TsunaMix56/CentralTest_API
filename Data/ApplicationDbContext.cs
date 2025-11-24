using Microsoft.EntityFrameworkCore;
using PropertyFavoritesApi.Models;

namespace PropertyFavoritesApi.Data;

/// <summary>
/// ApplicationDbContext - จัดการ In-Memory Database สำหรับ Development/Testing
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets - ตารางต่างๆ ในฐานข้อมูล
    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Favorite>()
            .HasKey(f => new { f.UserId, f.PropertyId });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Property)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "customer1", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new User { Id = 2, Username = "customer2", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new User { Id = 3, Username = "customer3", CreatedAt = DateTime.UtcNow.AddDays(-60) }
        );

        modelBuilder.Entity<Property>().HasData(
            new Property
            {
                Id = 1,
                Title = "คอนโดหรูใจกลางเมือง",
                Price = 3500000.00m,
                Location = "สาทร, กรุงเทพมหานคร",
                ImageUrl = "https://example.com/images/condo1.jpg",
                Description = "คอนโดมิเนียมสุดหรู 2 ห้องนอน 2 ห้องน้ำ พร้อมสระว่ายน้ำและฟิตเนส"
            },
            new Property
            {
                Id = 2,
                Title = "บ้านเดี่ยว 3 ชั้น ทำเลดี",
                Price = 8900000.00m,
                Location = "ลาดพร้าว, กรุงเทพมหานคร",
                ImageUrl = "https://example.com/images/house1.jpg",
                Description = "บ้านเดี่ยว 4 ห้องนอน 5 ห้องน้ำ พื้นที่ 250 ตร.ม. จอดรถได้ 3 คัน"
            },
            new Property
            {
                Id = 3,
                Title = "ทาวน์เฮ้าส์สไตล์โมเดิร์น",
                Price = 4200000.00m,
                Location = "รังสิต, ปทุมธานี",
                ImageUrl = "https://example.com/images/townhouse1.jpg",
                Description = "ทาวน์เฮ้าส์ 3 ห้องนอน 3 ห้องน้ำ ตกแต่งครบพร้อมอยู่"
            },
            new Property
            {
                Id = 4,
                Title = "คอนโดวิวทะเล ชั้น 15",
                Price = 5600000.00m,
                Location = "พัทยา, ชลบุรี",
                ImageUrl = "https://example.com/images/condo2.jpg",
                Description = "คอนโด 2 ห้องนอน วิวทะเลสวยงาม ใกล้ชายหาด"
            },
            new Property
            {
                Id = 5,
                Title = "อพาร์ทเมนท์ใกล้ BTS",
                Price = 2100000.00m,
                Location = "อารีย์, กรุงเทพมหานคร",
                ImageUrl = "https://example.com/images/apartment1.jpg",
                Description = "สตูดิโอขนาด 30 ตร.ม. เดินไป BTS 5 นาที"
            }
        );

        modelBuilder.Entity<Favorite>().HasData(
            new Favorite { UserId = 1, PropertyId = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Favorite { UserId = 1, PropertyId = 3, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Favorite { UserId = 2, PropertyId = 2, CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new Favorite { UserId = 2, PropertyId = 4, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Favorite { UserId = 3, PropertyId = 1, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Favorite { UserId = 3, PropertyId = 2, CreatedAt = DateTime.UtcNow.AddDays(-8) },
            new Favorite { UserId = 3, PropertyId = 5, CreatedAt = DateTime.UtcNow.AddDays(-1) }
        );
    }
}
