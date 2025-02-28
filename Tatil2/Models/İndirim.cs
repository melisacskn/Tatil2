using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class İndirim
    {
        [Key]
        public int OdaId { get; set; }  // OdaId'yi Primary Key olarak işaretliyoruz

        [ForeignKey(nameof(OdaId))]
        public Oda? Oda { get; set; }
        public DateOnly BaslangicTarihi { get; set; }
        public DateOnly BitisTarihi { get; set; }
        public decimal İndirimYuzdesi { get; set; }
    }
}
