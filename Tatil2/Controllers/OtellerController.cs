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

        public OtellerController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }


        public async Task<IActionResult> Incele(int id, DateTime BaslangicTarihi, DateTime BitisTarihi, int KisiSayisi)
        {
            var odalar = Tatildb.Oda
                            .Where(x => x.OtelId == id)
                            .Where(oda =>
                  oda.KisiSayisi >= KisiSayisi && oda.Rezervasyon.Count(r => r.BitisTarihi > BaslangicTarihi && r.BaslangicTarihi < BitisTarihi) < oda.OdaStok)


                           .ToList();

            if (odalar == null)
            {
                return NotFound();
            }
            ViewBag.BaslangicTarihi = BaslangicTarihi.ToString("yyyy-MM-dd");
            ViewBag.BitisTarihi = BitisTarihi.ToString("yyyy-MM-dd");
            ViewBag.KisiSayisi = KisiSayisi;


            return View(odalar);
        }

        //// yeni gelen kısım
        //[HttpPost]
        //public async Task<IActionResult> Index2(List<int>? TagId, decimal? MinPuan, decimal? MaxPuan)
        //{
        //    // Tüm tagları al
        //    var otelTag = await Tatildb.Tag.ToListAsync();

        //    // Tüm otelleri ve ilişkili verileri al
        //    var otellerQuery = Tatildb.Otel
        //        .Include(o => o.İlce)
        //        .Include(o => o.Tag)
        //        .Include(o => o.Odalar)
        //            .ThenInclude(oda => oda.Yorumlar)
        //        .AsQueryable();

        //    // Filtreleme işlemleri:
        //    if (TagId != null && TagId.Any())
        //    {
        //        otellerQuery = otellerQuery.Where(o => o.Tag.Any(t => TagId.Contains(t.Id)));
        //    }

        //    // Ortalama puanları al
        //    if (MinPuan.HasValue)
        //    {
        //        otellerQuery = otellerQuery
        //            .Where(o => o.Odalar
        //                .SelectMany(oda => oda.Yorumlar)
        //                .Average(p => p.Yorumlar) >= MinPuan.Value);
        //    }

        //    if (MaxPuan.HasValue)
        //    {
        //        otellerQuery = otellerQuery
        //            .Where(o => o.Odalar
        //                .SelectMany(oda => oda.Yorumlar)
        //                .Average(p => p.Yorumlar) <= MaxPuan.Value);
        //    }

        //    // Sonuçları al
        //    var oteller = await otellerQuery.ToListAsync();

        //    // ViewModel oluştur
        //    var viewModel = new OtelFiltreleViewModel
        //    {
        //        Oteller = oteller,
        //        otelTag = otelTag,
        //        SeciliTagIdler = TagId,
        //        MinPuan = MinPuan,
        //        MaxPuan = MaxPuan
        //    };

        //    return View(viewModel);
        //}
        public async Task<IActionResult> Index([FromForm] OtelFiltreleDTO otelFiltreleDTO)
        {

            var query = Tatildb.Otel.AsQueryable();
            if (!string.IsNullOrWhiteSpace(otelFiltreleDTO.Ara))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.Ad, otelFiltreleDTO.Ara)
                    || EF.Functions.Like(x.Konum, otelFiltreleDTO.Ara)
                    || EF.Functions.Like(x.İlce.Ad, otelFiltreleDTO.Ara)
                    || EF.Functions.Like(x.İlce.Sehir.Name, otelFiltreleDTO.Ara)
                    );
            }
            if (otelFiltreleDTO.MinPuan.HasValue)
            {
                query = query.Where(x => !x.Odalar.SelectMany(y => y.Yorumlar).Any() || // Yorum yoksa geçir
                                        x.Odalar.SelectMany(y => y.Yorumlar).Average(y => (decimal?)y.Puan) >= otelFiltreleDTO.MinPuan.Value);
            }

            if (otelFiltreleDTO.MaxPuan.HasValue)
            {
                query = query.Where(x => !x.Odalar.SelectMany(y => y.Yorumlar).Any() || // Yorum yoksa geçir
                                        x.Odalar.SelectMany(y => y.Yorumlar).Average(y => (decimal?)y.Puan) <= otelFiltreleDTO.MaxPuan.Value);
            }

            if (otelFiltreleDTO.TagId != null && otelFiltreleDTO.TagId.Any())
            {
                query = query.Where(x => x.Tag.Select(y => y.Id).Any(y => otelFiltreleDTO.TagId.Contains(y)));
            }

            query = query.Where(otel => otel.Odalar.Any(oda =>
               oda.KisiSayisi >= otelFiltreleDTO.KisiSayisi && oda.Rezervasyon.Count(r => (r.BitisTarihi < otelFiltreleDTO.BaslangicTarihi && r.BaslangicTarihi > otelFiltreleDTO.BitisTarihi)) < oda.OdaStok
             ))
                  .Include(o => o.İlce)
                  .Include(o => o.Odalar)
                  //.ToList()
                  ;

            // Tüm tagları al
            var otelTag = await Tatildb.Tag.ToListAsync();

            //if (oteller == null || !oteller.Any())
            //{
            //    return NotFound("Otel bulunamadı.");
            //}

            // ViewModel oluştur
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
