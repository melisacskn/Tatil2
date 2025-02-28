using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class KartBilgisi
    {
        [Key]
        public int RezervasyonId { get; set; }
        //[ForeignKey("RezervasyonId")]
        //TODO: Navigation Property -- EF Core
        [ForeignKey(nameof(RezervasyonId))]
        public Rezervasyon Rezervasyon { get; set; }
        public string Iban { get; set; }
        public string KartTarih { get; set; }
        public short Cvv { get; set; }
    }
}
