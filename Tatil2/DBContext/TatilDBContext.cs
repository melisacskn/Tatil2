using Microsoft.EntityFrameworkCore;
using Tatil2.Models;

namespace Tatil2.DBContext
{
    public class TatilDBContext : DbContext
    {
        public TatilDBContext(DbContextOptions<TatilDBContext> options) : base(options) { }

        public DbSet<Musteri> Musteri { get; set; }
        public DbSet<Otel> Otel { get; set; }
        public DbSet<İlce> İlce { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=ERYTRN065\\SQLEXPRESS; database=Tatil; Integrated Security=True; TrustServerCertificate=True;");
        }
    }
}
