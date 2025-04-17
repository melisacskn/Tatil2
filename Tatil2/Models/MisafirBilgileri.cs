using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class MisafirBilgileri
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime DogumTarihi { get; set; }
        public bool Cinsiyet { get; set; }
        [ForeignKey(nameof(RezervasyonId))]
        public Rezervasyon Rezervasyon { get; set; }
        public int RezervasyonId { get; set; }
        public string TC { get; set; }
        
    }
}
