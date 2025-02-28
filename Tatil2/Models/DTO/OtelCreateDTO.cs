using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models
{
    public class OtelCreateDTO
    {
        [MaxLength(100)]  // Maksimum 100 karakter
        public string Ad { get; set; }

        [MaxLength(500)]  // Maksimum 500 karakter
        public string Aciklama { get; set; }

        public IFormFile Poster { get; set; }

        [MaxLength(255)]  // Maksimum 255 karakter
        public string Konum { get; set; }
        public int İlceId { get; set; }
    }
}
