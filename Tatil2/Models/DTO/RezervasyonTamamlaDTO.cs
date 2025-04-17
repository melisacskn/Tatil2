using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models.DTO
{
    public class RezervasyonTamamlaDTO
    {
        public int OdaId { get; set; }
        public Oda Oda { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        [Required(ErrorMessage = "Kişi sayısı zorunludur")]
        public int KisiSayisi { get; set; }
        public int MusteriId { get; set; }
        public List<MisafirBilgileri> MisafirBilgileri { get; set; }
    }

 

}
