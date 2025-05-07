using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models
{
    public class OtelCreateDTO
    {
      
        public string Ad { get; set; }

        public string Aciklama { get; set; }

        public IFormFile Poster { get; set; }

        public string Konum { get; set; }

       
        public int İlceId { get; set; }

        public int SehirId { get; set; }

        // Oda bilgileri (isteğe bağlı olarak Required yapılabilir)
        public List<OdaCreateDTO> Odalar { get; set; } = new List<OdaCreateDTO>();

        // View'dan gelen checkbox'lardan seçilen tag id'ler
        public List<int> TagKategori { get; set; } = new List<int>();

       
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
