using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models
{
    public class Sehir
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
