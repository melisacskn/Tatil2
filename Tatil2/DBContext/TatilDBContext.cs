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
        public DbSet<Sehir> Sehir { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<OtelTag> OtelTag { get; set; }
        public DbSet<TagKategori> TagKategori { get; set; }
        public DbSet<Oda> Oda { get; set; }
        public DbSet<Rezervasyon> Rezervasyon { get; set; }
        public DbSet<Yorum> Yorum { get; set; }








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
            //modelBuilder.Entity<Otel>()
            //    .HasMany(x => x.Odalar)
            //    .WithOne(x => x.Otel)
            //    .HasForeignKey(x => x.OtelId);

            //modelBuilder.Entity<OtelTag>()
            //    .HasOne(ot => ot.Otel)
            //    .WithMany()  // Otel modeline eklenmiş bir koleksiyon varsa, ona referans vermelisiniz
            //    .HasForeignKey(ot => ot.OtelId);

            //modelBuilder.Entity<OtelTag>()
            //    .HasOne(ot => ot.Tag)
            //    .WithMany()  // Tag modeline eklenmiş bir koleksiyon varsa, ona referans vermelisiniz
            //    .HasForeignKey(ot => ot.TagId);

            //// Otel ve TagKategori arasında çoktan çoğa ilişkiyi tanımlıyoruz
            //modelBuilder.Entity<OtelTagKategori>()
            //    .HasKey(otk => new { otk.OtelId, otk.TagKategoriId });

            //modelBuilder.Entity<OtelTagKategori>()
            //    .HasOne(otk => otk.Otel)
            //    .WithMany(o => o.OtelTagKategoriler)
            //    .HasForeignKey(otk => otk.OtelId);

            //modelBuilder.Entity<OtelTagKategori>()
            //    .HasOne(otk => otk.TagKategori)
            //    .WithMany(t => t.OtelTagKategoriler)
            //    .HasForeignKey(otk => otk.TagKategoriId);
        }


    }
}
