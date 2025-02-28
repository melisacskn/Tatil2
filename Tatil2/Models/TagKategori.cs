using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models
{
    public class TagKategori
    {
        [Key]
        public int Id { get; set; }
        public string Ad { get; set; }
    }
}
