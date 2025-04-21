using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;
using System.Linq;
using System.Threading.Tasks;
using Tatil2.Models.DTO;

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
            // Verilen otel id'sine göre odaları filtreler. Oda kapasitesi ve tarihlerin uygunluğuna göre sorgu yapar.
            var odalar = Tatildb.Oda
                            .Where(x => x.OtelId == id) // Otel id'ye göre filtreleme
                            .Where(oda =>
                  oda.KisiSayisi >= KisiSayisi && // Oda kapasitesine göre filtreleme
                  oda.Rezervasyon.Count(r => r.BitisTarihi > BaslangicTarihi && r.BaslangicTarihi < BitisTarihi) < oda.OdaStok) // Tarihler çakışmıyorsa ve oda stokları yeterliyse
                           .ToList();

            // Eğer odalar bulunamazsa 404 döndürülür.
            if (odalar == null)
            {
                return NotFound();
            }

            // ViewBag üzerinden tarih ve kişi sayısı parametreleri view'a gönderilir.
            ViewBag.BaslangicTarihi = BaslangicTarihi.ToString("yyyy-MM-dd");
            ViewBag.BitisTarihi = BitisTarihi.ToString("yyyy-MM-dd");
            ViewBag.KisiSayisi = KisiSayisi;

            // Filtrelenen odalar view'a gönderilir
            return View(odalar);
        }

        // Otel filtreleme metodunu başlatır. 
        public async Task<IActionResult> Index([FromForm] OtelFiltreleDTO otelFiltreleDTO)
        {
            // Başlangıçta tüm otelleri alacak şekilde bir sorgu başlatılır.
            var query = Tatildb.Otel.AsQueryable();

            // Eğer arama parametresi varsa (otel adı, konum, ilçe adı, şehir adı ile eşleşme yapılır)
            if (!string.IsNullOrWhiteSpace(otelFiltreleDTO.Ara))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.Ad, otelFiltreleDTO.Ara) // Otel adı ile
                    || EF.Functions.Like(x.Konum, otelFiltreleDTO.Ara) // Konum ile
                    || EF.Functions.Like(x.İlce.Ad, otelFiltreleDTO.Ara) // İlçe adı ile
                    || EF.Functions.Like(x.İlce.Sehir.Name, otelFiltreleDTO.Ara) // Şehir adı ile
                    );
            }

            // MinPuan parametresi varsa, ortalama puanları bu değeri geçecek şekilde filtreler
            if (otelFiltreleDTO.MinPuan.HasValue)
            {
                query = query.Where(x => !x.Odalar.SelectMany(y => y.Yorumlar).Any() || // Yorum yoksa geçir
                                        x.Odalar.SelectMany(y => y.Yorumlar).Average(y => (decimal?)y.Puan) >= otelFiltreleDTO.MinPuan.Value);
            }

            // MaxPuan parametresi varsa, ortalama puanları bu değeri geçmeyecek şekilde filtreler
            if (otelFiltreleDTO.MaxPuan.HasValue)
            {
                query = query.Where(x => !x.Odalar.SelectMany(y => y.Yorumlar).Any() || // Yorum yoksa geçir
                                        x.Odalar.SelectMany(y => y.Yorumlar).Average(y => (decimal?)y.Puan) <= otelFiltreleDTO.MaxPuan.Value);
            }

            // TagId parametreleri varsa, otellerin tag'lerini filtreler
            if (otelFiltreleDTO.TagId != null && otelFiltreleDTO.TagId.Any())
            {
                query = query.Where(x => x.Tag.Select(y => y.Id).Any(y => otelFiltreleDTO.TagId.Contains(y)));
            }

            // Oda kapasitesine göre ve tarih aralıklarına göre uygun odalar olmalı
            query = query.Where(otel => otel.Odalar.Any(oda =>
               oda.KisiSayisi >= otelFiltreleDTO.KisiSayisi && // Oda kapasitesine göre filtre
               oda.Rezervasyon.Count(r => (r.BitisTarihi < otelFiltreleDTO.BaslangicTarihi && r.BaslangicTarihi > otelFiltreleDTO.BitisTarihi)) < oda.OdaStok // Tarihler arasında rezervasyon çakışması olmamalı
             ))
                  .Include(o => o.İlce) // İlçe bilgisi dahil edilir
                  .Include(o => o.Odalar); // Oda bilgisi dahil edilir

            // Tüm tagları al
            var otelTag = await Tatildb.Tag.ToListAsync();

            // Filtreleme sonrası sonuçları view'a gönder
            var viewModel = new OtelFiltreleDTO
            {
                Oteller = query.ToList(),
                OtelTag = otelTag,
                TagId = otelFiltreleDTO.TagId,
                Ara = otelFiltreleDTO.Ara,
                BaslangicTarihi = otelFiltreleDTO.BaslangicTarihi,
                BitisTarihi = otelFiltreleDTO.BitisTarihi,
                KisiSayisi = otelFiltreleDTO.KisiSayisi,
                MinPuan = otelFiltreleDTO.MinPuan,
                MaxPuan = otelFiltreleDTO.MaxPuan
            };

            return View(viewModel);
        }
    }
}
