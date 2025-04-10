using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Tatil2.Controllers
{
    public class OtellerController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public OtellerController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }


        public IActionResult Index(DateTime BaslangicTarihi, DateTime BitisTarihi, int KisiSayisi)
        {
            // BURADA OTELLERİ GÖRECEK, ODALARA GEREK YOK. OTEL DETAYINA GİDİNCE ODALARI GÖRECEK (İNCELE SAYFASI...)
            var oteller = Tatildb.Otel
                                //.Include(o => o.Odalar)
                                //.Where(otel => otel.Odalar.Any())
                                .AsQueryable();

            // boşsa 0 gelir, filtrede sorun olmaz
            //var finalKisiSayisi = KisiSayisi.GetValueOrDefault();

            oteller = oteller.Where(otel => otel.Odalar.Any(oda =>
              oda.KisiSayisi >= KisiSayisi && oda.Rezervasyon.Count(r => (r.BitisTarihi < BaslangicTarihi && r.BaslangicTarihi > BitisTarihi)) < oda.OdaStok
            ));


            ViewBag.BaslangicTarihi = BaslangicTarihi;
            ViewBag.BitisTarihi = BitisTarihi;
            ViewBag.KisiSayisi = KisiSayisi;

            var model = oteller.ToList();
            return View(model);
        }

        public async Task<IActionResult> Incele(int id, DateTime BaslangicTarihi, DateTime BitisTarihi, int KisiSayisi)
        {
            var odalar = Tatildb.Oda
                            .Where(x => x.OtelId == id)
                            .Where(oda =>
                  oda.KisiSayisi >= KisiSayisi && oda.Rezervasyon.Count(r => (r.BitisTarihi < BaslangicTarihi && r.BaslangicTarihi > BitisTarihi)) < oda.OdaStok)
                           //.Include(x=> x.)
                           //.Include(o => o.İlce)
                           //.Include(o => o.Tag)
                           //.Include(o => o.Odalar)
                           .ToList();

            if (odalar == null)
            {
                return NotFound();
            }
            ViewBag.BaslangicTarihi = BaslangicTarihi.ToString("yyyy-MM-dd");
            ViewBag.BitisTarihi = BitisTarihi.ToString("yyyy-MM-dd");


            return View(odalar);
        }
    }
}
