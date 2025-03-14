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
        public int SehirId { get; set; }

        // Odalar'ı ekliyoruz
        public List<OdaCreateDTO> Odalar { get; set; }
        public List<int> SelectedTagId { get; set; }
        public DateOnly BaslangicTarihi { get; set; }
        public DateOnly BitisTarihi { get; set; }
       
    }

    public class OdaCreateDTO
    {
        public string OdaAd { get; set; }
        public string OdaAciklama { get; set; }
        public decimal OdaFiyat { get; set; }
        public short KisiSayisi { get; set; }
        public int OdaStok { get; set; }
        public int OtelId { get; set; }
        public IFormFile OdaFoto { get; set; }
        
    }

}
