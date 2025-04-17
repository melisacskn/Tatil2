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

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Yorum en az 3, en fazla 100 karakter olmalıdır.")]
        public string Yazi { get; set; }

        [Range(1, 10, ErrorMessage = "Yorumlar 1 ile 10 arasında olmalıdır.")]
        public decimal Puan { get; set; }

        public int OtelId { get; set; }

        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }
        public int OdaId { get; set; }

        [ForeignKey(nameof(OdaId))]
        public Oda Oda { get; set; } 
     





    }
}
