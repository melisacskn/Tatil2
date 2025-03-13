using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class OdaFotograf
    {
        public int Id { get; set; }
        public int OdaId { get; set; }
        [ForeignKey (nameof (OdaId))]
        public Oda Oda  { get; set; }
        public int OtelId { get; set; }
        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }
        public string Fotograf { get; set; }
    }
}
