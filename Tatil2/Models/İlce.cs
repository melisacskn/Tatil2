using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class İlce
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey(nameof(SehirId))]
        public Sehir Sehir { get; set; }
        public int SehirId { get; set; }
        public List<Otel> Otel { get; set; }
        

    }
}
