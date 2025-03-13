using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tatil2.Models;

namespace Tatil2.Models
{
    public class OtelTag
    {
        [ForeignKey(nameof(OtelId))]
        public int OtelId { get; set; }
        public Otel Otel { get; set; }  // Otel ile ilişki
        [ForeignKey(nameof(TagId))]
        public int TagId { get; set; }
        public Tag Tag { get; set; }  // Tag ile ilişki
    }
}
