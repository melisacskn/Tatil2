using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Rezervasyon
    {
        public int Id { get; set; }

        public int KartBilgisiId { get; set; }
        
        [ForeignKey(nameof(KartBilgisiId))]
        public KartBilgisi KartBilgisi { get; set; }

        public int MusteriId { get; set; }  
        [ForeignKey(nameof(MusteriId))]
        public Musteri Musteri { get; set; }

        public int OdaId { get; set; }
        [ForeignKey(nameof(OdaId))]
        public Oda Oda { get; set; }

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        
    }
}
