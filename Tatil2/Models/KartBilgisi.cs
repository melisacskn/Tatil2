using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tatil2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class KartBilgisi
    {
        [Key]  // Birincil anahtar
        public int Id { get; set; }

        public int RezervasyonId { get; set; }  // Foreign key

        [ForeignKey(nameof(RezervasyonId))]
        public Rezervasyon Rezervasyon { get; set; }  // İlişkili Rezervasyon

        [Required]
        [RegularExpression(@"[A-Z]{2}\d{2}[A-Z0-9]{11,30}", ErrorMessage = "Geçerli bir IBAN girin.")]
        public string Iban { get; set; }  // IBAN

        [Required]
        [DataType(DataType.Date)]
        public DateTime KartTarih { get; set; }  // Kartın son kullanma tarihi (string yerine DateTime)

        [Required]
        [Range(100, 999, ErrorMessage = "CVV 3 haneli olmalıdır.")]
        public short Cvv { get; set; }  // CVV kodu (3 haneli olmalı)
        public DateTime BitisTarihi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public int OdaId{ get; set; }
        [ForeignKey(nameof(OdaId))]
        public Oda Oda { get; set; }
    }

}
