using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tatil2.Models;

namespace Tatil2.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int TagKategoriId { get; set; }
        [ForeignKey(nameof(TagKategoriId))]
        public TagKategori TagKategori { get; set; }
        //public ICollection<OtelTag> OtelTag { get; set; }  // OtelTag ile ilişki
        public ICollection<Otel> Otel { get; set; }
    }
}
