using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;
using System.Linq;
using System.Threading.Tasks;
using Tatil2.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Tatil2.Controllers
{
    public class OtellerController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        // Constructor: TatilDBContext nesnesi ile controller'ı initialize eder
        public OtellerController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Otel odalarını incelemek için kullanılan metod
        // id, BaslangicTarihi, BitisTarihi, KisiSayisi parametreleri alır.

        public async Task<IActionResult> Incele(int id, DateTime BaslangicTarihi, DateTime BitisTarihi, int KisiSayisi)
        {
            var odalar = Tatildb.Oda
                .Include(o => o.Otel)               
                .Include(o => o.Yorum)
                .ThenInclude(y => y.Musteri)     
                .Where(x => x.OtelId == id)
                .Where(oda =>
                    oda.KisiSayisi >= KisiSayisi &&
                    oda.Rezervasyon.Count(r => r.BitisTarihi > BaslangicTarihi && r.BaslangicTarihi < BitisTarihi) < oda.OdaStok)
                .ToList();

            if (odalar == null || !odalar.Any())
            {
                return NotFound();
            }

            // Ortalama puanı hesapla
            var yorumlar = odalar
                .Where(o => o.Yorum != null)
                .SelectMany(o => o.Yorum)
                .Where(y => y.OtelId == id)
                .ToList();

            var ortalamaPuan = yorumlar.Any() ? yorumlar.Average(y => y.Puan) : 0;

            // ViewBag'e aktar
            ViewBag.OrtalamaPuan = ortalamaPuan.ToString("0.0");

            ViewBag.BaslangicTarihi = BaslangicTarihi.ToString("yyyy-MM-dd");
            ViewBag.BitisTarihi = BitisTarihi.ToString("yyyy-MM-dd");
            ViewBag.KisiSayisi = KisiSayisi;

            return View(odalar);
        }


        // Otel filtreleme metodunu başlatır.

        public async Task<IActionResult> Index([FromForm] OtelFiltreleDTO otelFiltreleDTO)
        {
            int sayfaBasinaOtel = 2; // Sayfa başına 2 otel
            var query = Tatildb.Otel.AsQueryable();

            // Filtreleme işlemleri
            if (!string.IsNullOrWhiteSpace(otelFiltreleDTO.Ara))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.Ad, otelFiltreleDTO.Ara) ||
                    EF.Functions.Like(x.Konum, otelFiltreleDTO.Ara) ||
                    EF.Functions.Like(x.İlce.Ad, otelFiltreleDTO.Ara) ||
                    EF.Functions.Like(x.İlce.Sehir.Name, otelFiltreleDTO.Ara)
                );
            }

            if (otelFiltreleDTO.MinPuan.HasValue)
            {
                query = query.Where(x => !x.Odalar.SelectMany(y => y.Yorum).Any() ||
                                        x.Odalar.SelectMany(y => y.Yorum).Average(y => (decimal?)y.Puan) >= otelFiltreleDTO.MinPuan.Value);
            }

            if (otelFiltreleDTO.MaxPuan.HasValue)
            {
                query = query.Where(x => !x.Odalar.SelectMany(y => y.Yorum).Any() ||
                                        x.Odalar.SelectMany(y => y.Yorum).Average(y => (decimal?)y.Puan) <= otelFiltreleDTO.MaxPuan.Value);
            }

            if (otelFiltreleDTO.TagId != null && otelFiltreleDTO.TagId.Any())
            {
                query = query.Where(x => x.Tag.Select(y => y.Id).Any(y => otelFiltreleDTO.TagId.Contains(y)));
            }

            // Oda kapasitesine göre filtreleme
            query = query.Where(otel => otel.Odalar.Any(oda =>
                oda.KisiSayisi >= otelFiltreleDTO.KisiSayisi &&
                oda.Rezervasyon.Count(r => (r.BitisTarihi < otelFiltreleDTO.BaslangicTarihi && r.BaslangicTarihi > otelFiltreleDTO.BitisTarihi)) < oda.OdaStok
            ))
            .Include(o => o.İlce)
            .Include(o => o.Odalar);

            // Otel sayısını al
            var toplamOtelSayisi = await query.CountAsync();

            // Sayfa başına 2 otel olacak şekilde sorgu
            var oteller = await query
                .Skip((otelFiltreleDTO.Sayfa - 1) * sayfaBasinaOtel)  // Geçen sayfalardaki otelleri atla
                .Take(sayfaBasinaOtel)               // Bu sayfada 2 otel al
                .ToListAsync();

            // Sayfalama bilgilerini oluştur
            var sayfalar = (int)Math.Ceiling(toplamOtelSayisi / (double)sayfaBasinaOtel);

            // Filtreleme ve sayfalama bilgilerini view'a gönder
            var viewModel = new OtelFiltreleDTO
            {
                Oteller = oteller,
                Sayfa = otelFiltreleDTO.Sayfa,
                OtelTag = await Tatildb.Tag.ToListAsync(),
                TagId = otelFiltreleDTO.TagId,
                Ara = otelFiltreleDTO.Ara,
                BaslangicTarihi = otelFiltreleDTO.BaslangicTarihi,
                BitisTarihi = otelFiltreleDTO.BitisTarihi,
                KisiSayisi = otelFiltreleDTO.KisiSayisi,
                MinPuan = otelFiltreleDTO.MinPuan,
                MaxPuan = otelFiltreleDTO.MaxPuan,
                ToplamSayfa = sayfalar // Toplam sayfa bilgisini view model'e ekledik
            };

            return View(viewModel);
        }
    }
}
