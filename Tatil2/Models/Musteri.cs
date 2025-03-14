using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Musteri
    {
        [Key]
        public int Id { get; set; }  

        [Required]
        public string Ad { get; set; }
        public string Soyad { get; set; }

        [Required]
        [EmailAddress]
        public string Mail { get; set; }

        [MaxLength(10)]
        public string? Telefon { get; set; }

        [MaxLength(11)]
        public string TC { get; set; }
        public string Sifre { get; set; }
        public bool Cinsiyet { get; set; }
        public ICollection<KartBilgisi> KartBilgisi{ get; set; }
        public int MusteriId { get; set; }  // Müşteri ile ilişkilendiriyoruz
        [ForeignKey(nameof(MusteriId))]
        public Musteri musteri { get; set; }
    }
}
