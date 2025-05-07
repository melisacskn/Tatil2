using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Otel
    {
        public int Id { get; set; }

        [MaxLength(100)]  // Maksimum 100 karakter
        public string Ad { get; set; }

        [MaxLength(500)]  // Maksimum 500 karakter
        public string Aciklama { get; set; }

        [MaxLength(255)]  // Maksimum 255 karakter
        public string Poster { get; set; }

        [MaxLength(255)]  // Maksimum 255 karakter
        public string Konum { get; set; }

        public int İlceId { get; set; }
        [ForeignKey(nameof(İlceId))]
        public İlce İlce { get; set; }

        public ICollection<Tag> Tag { get; set; } = [];
        //public ICollection<OtelTag> OtelTag { get; set; }
        public virtual ICollection<Oda> Odalar { get; set; }
        public ICollection<OtelTag> OtelTagList { get; set; } = new List<OtelTag>();

        //public virtual ICollection<Rezervasyon> Rezervasyon { get; set; }

    }
}
