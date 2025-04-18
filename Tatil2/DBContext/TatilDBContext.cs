using Microsoft.EntityFrameworkCore;
using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.DBContext
{
    public class TatilDBContext : DbContext
    {
        public TatilDBContext(DbContextOptions<TatilDBContext> options) : base(options) { }

        public DbSet<Musteri> Musteri { get; set; }
        public DbSet<Otel> Otel { get; set; }
        public DbSet<İlce> İlce { get; set; }
        public DbSet<Sehir> Sehir { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<OtelTag> OtelTag { get; set; }
        public DbSet<TagKategori> TagKategori { get; set; }
        public DbSet<Oda> Oda { get; set; }
        public DbSet<Rezervasyon> Rezervasyon { get; set; }
        public DbSet<Yorum> Yorum { get; set; }
        public DbSet<MisafirBilgileri> MisafirBilgileri { get; set; }
        //public DbSet<RezervasyonTamamlaDTO> RezervasyonTamamlaDTO { get; set; }

        // OnConfiguring metodunda zaman aşımı ayarını yapıyoruz
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               
                optionsBuilder.UseSqlServer(
                    "YourConnectionString",
                    sqlOptions => sqlOptions.CommandTimeout(180) 
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OtelTag>()
                 .HasKey(x => new { x.OtelId, x.TagId });

            modelBuilder.Entity<Otel>()
                .HasMany(x => x.Tag)
                .WithMany(x => x.Otel)
                .UsingEntity<OtelTag>();

            modelBuilder.Entity<Rezervasyon>()
                .Property(x => x.BaslangicTarihi)
                .HasColumnType("smalldatetime");

            modelBuilder.Entity<Rezervasyon>()
                .Property(x => x.BitisTarihi)
                .HasColumnType("smalldatetime");

        }
    }
}
