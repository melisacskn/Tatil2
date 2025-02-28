using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Fotograf
    {
        [Key]
        public int OtelId { get; set; }
        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }
        public string Foto { get; set; }
    }
}
