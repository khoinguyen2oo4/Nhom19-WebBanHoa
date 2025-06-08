using Microsoft.EntityFrameworkCore;


namespace Nhom19_WebBanHoa.Models
{
    public class FlowerContext : DbContext
    {
        public FlowerContext(DbContextOptions<FlowerContext> options) : base(options)
        {
        }

        public DbSet<Flower> Flowers { get; set; }
    }
}
