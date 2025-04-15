
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tatil2.Models
{
    public class OtelFiltreleViewModel
    {
        public List<Otel> Oteller { get; set; }
        public List<Tag> OtelTag { get; set; }

        public List<int>? SeciliTagIdler { get; set; }

        [Range(0, 10)]
        public decimal? MinPuan { get; set; }

        [Range(0, 10)]
        public decimal? MaxPuan { get; set; }
    }
}
