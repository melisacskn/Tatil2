using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Oda
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }
        public string OtelId { get; set; }
        public string Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public short KisiSayisi { get; set; }
    }
}
