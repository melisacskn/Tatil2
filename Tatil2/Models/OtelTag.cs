using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class OtelTag
    {
        public int Id { get; set; }
        [ForeignKey(nameof(OtelId))]
        public Otel Otel { get; set; }
        public int OtelId { get; set; }
        [ForeignKey(nameof(TagId))]
        public string TagTag { get; set; }
        public int TagId { get; set; }

    }
}
