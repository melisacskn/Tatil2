using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models.DTO
{
    public class OtelFiltreleDTO
    {
        public string? Ara { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? KisiSayisi { get; set; }
        public List<int>? TagId { get; set; } = [];
        [Range(0, 10)]
        public decimal? MinPuan { get; set; }
        [Range(0, 10)]
        public decimal? MaxPuan { get; set; }

        public List<Otel>? Oteller { get; set; } = [];
        public List<Tag> OtelTag { get; set; } = [];
    }
}
