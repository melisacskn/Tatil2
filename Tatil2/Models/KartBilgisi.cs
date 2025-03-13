using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class KartBilgisi
    {
        [Key]  // Bu, Id alanını birincil anahtar olarak işaretler
        public int Id { get; set; }  // Anahtar alanı, her KartBilgisi nesnesi için benzersiz bir kimlik verir

        public int RezervasyonId { get; set; }  // Foreign key olarak RezervasyonId

        [ForeignKey(nameof(RezervasyonId))]
        public Rezervasyon Rezervasyon { get; set; }  // Navigation property: Bir KartBilgisi'nin ilişkili olduğu Rezervasyon

        public string Iban { get; set; }  // IBAN numarası
        public string KartTarih { get; set; }  // Kartın son kullanma tarihi
        public short Cvv { get; set; }  // CVV kodu
    }
}
