using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom19_WebBanHoa.Models
{
    public class FlowerContext : DbContext
    {
        public FlowerContext(DbContextOptions<FlowerContext> options) : base(options)
        {
        }

        public DbSet<Flower> Flowers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify precision for Gia property
            modelBuilder.Entity<Flower>()
                .Property(f => f.Gia)
                .HasPrecision(18, 2); // 18 digits, 2 decimal places

            // Đảm bảo các trường User nullable/bắt buộc đúng với model
            modelBuilder.Entity<User>()
                .Property(u => u.Avatar)
                .IsRequired(false); // Avatar nullable

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired(false); // Role nullable

            // Không cần mapping cho ConfirmPassword vì đã có [NotMapped]

            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    FullName = "Administrator",
                    Email = "admin@gmail.com",
                    PhoneNumber = "0123456789",
                    Password = "admin123", // Đơn giản, bạn nên hash mật khẩu trong thực tế
                    Avatar = null,
                    Role = "admin"
                }
            );
        }
    }
}
