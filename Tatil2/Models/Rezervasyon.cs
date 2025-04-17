using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace Tatil2.Models
{
    public class Rezervasyon
    {
        public int Id { get; set; }

        public int MusteriId { get; set; }
        [ForeignKey(nameof(MusteriId))]
        public Musteri Musteri { get; set; }
        public virtual ICollection<MisafirBilgileri> MisafirBilgileri { get; set; } = [];

        public int OdaId { get; set; }
        [ForeignKey(nameof(OdaId))]
        public Oda Oda { get; set; }

        //public string KartSahibiAdi { get; set; }

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }



        //public string KartNo { get; set; }
        //public string KartTarih { get; set; }
        //public string Cvv { get; set; }
    }

    public class RezervasyonCreateDTO
    {

        //public int? KartBilgisiId { get; set; }
        public int OdaId { get; set; }
        public List<MisafirBilgileriCreateDTO> MisafirBilgileri { get; set; } = [];

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        [Required]
        public string KartSahibiAdi { get; set; }

        [Required]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Geçerli bir kredi kartı numarası girin (13-19 haneli sayılar).")]
        public string KartNo { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Geçerli bir tarih formatı girin (MM/YY).")]
        public string KartTarih { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV 3 haneli olmalıdır.")]
        public string Cvv { get; set; }
        //public virtual ICollection<Yorum> Yorum { get; set; }

    }

    public class MisafirBilgileriCreateDTO
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime DogumTarihi { get; set; }
        public bool Cinsiyet { get; set; }
        public string TC { get; set; }

    }
}