using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Yorum
    {
        public int Id { get; set; }
        [ForeignKey(nameof(MusteriId))]
        public Musteri Musteri { get; set; }
        public int MusteriId { get; set; }
        public string Yazi { get; set; }
        public decimal Puan { get; set; }
        public int OtelId { get; set; }
        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }
    }
}
