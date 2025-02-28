using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public String Ad { get; set; }
        [ForeignKey(nameof(KategoriId))]
        public TagKategori Kategori{ get; set; }
        public int KategoriId { get; set; }
    }
}
