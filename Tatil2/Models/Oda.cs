using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Oda
    {
        public int Id { get; set; } 
        public string OdaAd { get; set; }
        public string OdaAciklama { get; set; }
        public decimal OdaFiyat { get; set; }
        public short KisiSayisi { get; set; }
        public int OdaStok { get; set; }
        public int OtelId { get; set; }
        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }  
        public string? OdaFoto { get; set; }
        public virtual ICollection<Rezervasyon> Rezervasyon { get; set; }
        public virtual ICollection<Yorum> Yorumlar { get; set; }
     

    }

}
