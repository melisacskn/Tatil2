using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models.DTO
{
    public class OtelFiltreleDTO
    {
        public string? Ara { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? KisiSayisi { get; set; }
        public List<int>? TagId { get; set; } = new List<int>();
        [Range(0, 10)]
        public decimal? MinPuan { get; set; }
        [Range(0, 10)]
        public decimal? MaxPuan { get; set; }

        public List<Otel>? Oteller { get; set; } = [];
        public List<Oda>? Oda { get; set; } = [];
        public List<Tag> OtelTag { get; set; }
        public int Sayfa { get; set; } = 1;
        public int ToplamSayfa { get; set; }
    }
}
