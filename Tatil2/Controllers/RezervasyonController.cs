using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public RezervasyonController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        
        [HttpGet]
        public IActionResult Rezervasyon([FromQuery] RezervasyonDTO rezervasyon)
        {
            var yorumlar = Tatildb.Yorum
                .Where(y => y.OdaId == rezervasyon.OdaId)
                .Include(y => y.Musteri)
                .ToList();

            ViewBag.Yorumlar = yorumlar;

            return View(rezervasyon);
        }

        [HttpGet]
        public JsonResult Oda(int odaId, DateTime baslangic, DateTime bitis)
        {
            bool odaDolu = Tatildb.Rezervasyon.Any(r => r.OdaId == odaId &&
                ((baslangic >= r.BaslangicTarihi && baslangic < r.BitisTarihi) ||
                 (bitis > r.BaslangicTarihi && bitis <= r.BitisTarihi) ||
                 (baslangic <= r.BaslangicTarihi && bitis >= r.BitisTarihi)));

            return Json(new { musait = !odaDolu });
        }

        
        [HttpGet]
        public IActionResult YorumYap(int odaId)
        {
            string userJson = HttpContext.Session.GetString("login");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("SignIn", "Giris");

            var user = JsonConvert.DeserializeObject<Musteri>(userJson);
            var oda = Tatildb.Oda.FirstOrDefault(o => o.Id == odaId);

            if (oda == null)
                return NotFound();

            ViewBag.Oda = oda;
            ViewBag.KullaniciAdi = user.Ad;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumYap(int odaId, string yorumMetni, int puan)
        {
            string userJson = HttpContext.Session.GetString("login");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("SignIn", "Giris");

            var user = JsonConvert.DeserializeObject<Musteri>(userJson);
            var oda = await Tatildb.Oda.FirstOrDefaultAsync(o => o.Id == odaId);

            if (oda == null)
                return NotFound();

            // Giriş validasyonları
            if (string.IsNullOrWhiteSpace(yorumMetni) || yorumMetni.Length < 3 || yorumMetni.Length > 100 || puan < 1 || puan > 10)
            {
                TempData["ErrorMessage"] = "Yorum ya da puan alanı hatalı. Lütfen kontrol ediniz.";
                return RedirectToAction("YorumYap", new { odaId });
            }

            var yorum = new Yorum
            {
                OdaId = oda.Id,
                OtelId = oda.OtelId,
                MusteriId = user.Id,
                Yazi = yorumMetni,
                Puan = puan
            };

            Tatildb.Yorum.Add(yorum);
            await Tatildb.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yorumunuz başarıyla kaydedildi.";
            return RedirectToAction("Rezervasyon", new RezervasyonDTO { OdaId = oda.Id });
        }

        // ODA DETAY SAYFASI
        public IActionResult OdaDetay(int odaId)
        {
            var oda = Tatildb.Oda.FirstOrDefault(o => o.Id == odaId);

            if (oda == null)
                return NotFound();

            var yorumlar = Tatildb.Yorum
                .Where(y => y.OdaId == odaId)
                .Include(y => y.Musteri)
                .ToList();

            ViewBag.Oda = oda;
            ViewBag.Yorumlar = yorumlar;

            return View();
        }

        // REZERVASYON OLUŞTUR - POST
        [HttpPost]
        public IActionResult Rezervasyon(RezervasyonCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Message = "Giriş bilgilerini kontrol ediniz" });
            }

            try
            {
                string userJson = HttpContext.Session.GetString("login");
                if (string.IsNullOrWhiteSpace(userJson))
                    return Json(new { Message = "Oturum süresi doldu, lütfen tekrar giriş yapınız." });

                var user = JsonConvert.DeserializeObject<Musteri>(userJson);

                Rezervasyon rezervasyon = new Rezervasyon
                {
                    MusteriId = user.Id,
                    OdaId = model.OdaId,
                    BaslangicTarihi = model.BaslangicTarihi,
                    BitisTarihi = model.BitisTarihi
                };

                Tatildb.Rezervasyon.Add(rezervasyon);
                Tatildb.SaveChanges();

                TempData["SuccessMessage"] = "Rezervasyonunuz başarıyla tamamlandı!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rezervasyon hatası: {ex.Message}");
                return Json(new { Message = "Rezervasyon işlemi sırasında bir hata oluştu." });
            }
        }
    }
}
