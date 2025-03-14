using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public RezervasyonController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        public IActionResult Rezervasyon()
        {
            return View(new KartBilgisi());
        }

        [HttpGet]
        public JsonResult OdaMusait(int odaId, DateTime baslangic, DateTime bitis)
        {
            var rezervasyon = Tatildb.Rezervasyon.Any(r => r.OdaId == odaId && (
                (baslangic >= r.BaslangicTarihi && baslangic <= r.BitisTarihi) ||
                (bitis >= r.BaslangicTarihi && bitis <= r.BitisTarihi) ||
                (baslangic <= r.BaslangicTarihi && bitis >= r.BitisTarihi)
            ));

            return Json(new { musait = !rezervasyon });
        }

        [HttpPost]
        public IActionResult Rezervasyon(Rezervasyon rezervasyon, KartBilgisi kartBilgisi)
        {
            if (!ModelState.IsValid)
            {
                return View(kartBilgisi);
            }

            if (kartBilgisi.KartTarih < DateTime.Today)
            {
                ModelState.AddModelError("KartTarih", "Kartın son kullanma tarihi geçmiş olamaz.");
                return View(kartBilgisi);
            }

            bool odaDoluMu = Tatildb.Rezervasyon.Any(r =>
                r.OdaId == rezervasyon.OdaId &&
                ((r.BaslangicTarihi <= rezervasyon.BitisTarihi && r.BaslangicTarihi >= rezervasyon.BaslangicTarihi) ||
                 (r.BitisTarihi >= rezervasyon.BaslangicTarihi && r.BitisTarihi <= rezervasyon.BitisTarihi)));

            if (odaDoluMu)
            {
                ModelState.AddModelError("", "Bu oda seçilen tarihler arasında dolu. Lütfen farklı bir tarih seçin.");
                return View(rezervasyon);
            }

            try
            {
                // Rezervasyonu ekle
                Tatildb.Rezervasyon.Add(rezervasyon);
                Tatildb.SaveChanges(); // İlk SaveChanges çağırılıyor

                // Kart bilgisine rezervasyon ID atanıyor
                kartBilgisi.RezervasyonId = rezervasyon.Id;
                Tatildb.KartBilgisi.Add(kartBilgisi);
                Tatildb.SaveChanges(); // Kart bilgisi ekleniyor

                TempData["SuccessMessage"] = "Ödeme bilgileri kaydedildi!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Rezervasyon işlemi sırasında bir hata oluştu: " + ex.Message);
                return View(rezervasyon);
            }
        }
    }
}
