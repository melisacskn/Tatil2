using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;
using Tatil2.DBContext;
using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Controllers
{
    public class RezervasyonController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        public RezervasyonController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Oda durumu kontrolü için bir metod
        // Belirtilen oda ve tarih aralığına göre odanın dolu olup olmadığını kontrol eder.
        [HttpGet]
        public JsonResult Oda(int odaId, DateTime baslangic, DateTime bitis)
        {
            bool odaDolu = Tatildb.Rezervasyon.Any(r => r.OdaId == odaId &&
                ((baslangic >= r.BaslangicTarihi && baslangic < r.BitisTarihi) ||
                 (bitis > r.BaslangicTarihi && bitis <= r.BitisTarihi) ||
                 (baslangic <= r.BaslangicTarihi && bitis >= r.BitisTarihi)));

            // Eğer oda doluysa "musait" false döner, değilse true döner.
            return Json(new { musait = !odaDolu });
        }

        // Rezervasyon sayfası için GET metodu
        // Rezervasyon bilgilerini ve oda ile ilgili yorumları alır ve gösterir.
        [HttpGet]
        public IActionResult Rezervasyon([FromQuery] RezervasyonDTO rezervasyon)
        {
            var yorumlar = Tatildb.Yorum
                .Where(y => y.OdaId == rezervasyon.OdaId)
                .Include(y => y.Musteri)
                .ToList();

            // Yorumları ViewBag üzerinden view'a aktarır
            ViewBag.Yorumlar = yorumlar;

            return View(rezervasyon);
        }
    
        // Kullanıcı bir oda ile ilgili yorum yazdığında çağrılır
        // Yorum metninin geçerliliğini kontrol eder ve geçerliyse kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumYap(int odaId, string yorumMetni, int puan)
        {
            if (Musteri == null)
                return RedirectToAction("SignIn", "Giris");

            var oda = await Tatildb.Oda.FirstOrDefaultAsync(o => o.Id == odaId);
            if (oda == null)
                return NotFound();

            // Yorum metni ve puan kontrolü
            if (string.IsNullOrWhiteSpace(yorumMetni) || yorumMetni.Length < 3 || yorumMetni.Length > 100 || puan < 1 || puan > 10)
            {
                TempData["ErrorMessage"] = "Yorum ya da puan alanı hatalı. Lütfen kontrol ediniz.";
                return RedirectToAction("Rezervasyon", new { OdaId = odaId });
            }

            var yorum = new Yorum
            {
                OdaId = oda.Id,
                OtelId = oda.OtelId,
                MusteriId = Musteri.Id,
                Yazi = yorumMetni,
                Puan = puan
            };

            // Yorum kaydedilir
            Tatildb.Yorum.Add(yorum);
            await Tatildb.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yorumunuz başarıyla kaydedildi.";
            return RedirectToAction("Rezervasyon", new
            {
                OdaId = oda.Id,
                BaslangicTarihi = DateTime.Now.ToString("yyyy-MM-dd"),
                BitisTarihi = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
            });

        }

        // Rezervasyon işlemini tamamlamak için odanın bilgilerini alır
        [Authorize]
        [HttpGet]
        public IActionResult Tamamla(int odaId, DateTime baslangic, DateTime bitis, int kisiSayisi)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false, message = "Form verileri geçersiz." });
            }
            var oda = Tatildb.Oda.Include(o => o.Otel).FirstOrDefault(o => o.Id == odaId);
            if (oda == null) return NotFound();

            // Rezervasyon tamamlanması için bir ViewModel oluşturulur
            var viewModel = new RezervasyonTamamlaDTO
            {
                OdaId = oda.Id,
                Oda = oda,
                BaslangicTarihi = baslangic,
                KisiSayisi = kisiSayisi,
                BitisTarihi = bitis,
                MusteriId = base.Musteri.Id,
                MisafirBilgileri = new List<MisafirBilgileri>()
            };

            // Kişi sayısına göre misafir bilgileri eklenir
            for (int i = 0; i < kisiSayisi; i++)
            {
                viewModel.MisafirBilgileri.Add(new MisafirBilgileri());
            }

            return View(viewModel);
        }

        // Rezervasyon işlemi tamamlandığında çağrılır
        // Kullanıcının girdiği bilgilerle rezervasyon kaydeder.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Tamamla(RezervasyonCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları eksiksiz doldurun.";
                return Json(new
                {
                    isSuccess = false,
                    message = "Lütfen tüm alanları eksiksiz doldurun."
                });
            }

            try
            {
                
                var oda = Tatildb.Oda.FirstOrDefault(o => o.Id == model.OdaId);
                if (oda == null)
                {
                    return Json(new { isSuccess = false, message = "Oda bulunamadı." });
                }

                int cakisanRezervasyonSayisi = Tatildb.Rezervasyon
                    .Count(r => r.OdaId == model.OdaId &&
                                !(r.BitisTarihi <= model.BaslangicTarihi || r.BaslangicTarihi >= model.BitisTarihi));

                if (cakisanRezervasyonSayisi >= oda.OdaStok)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Seçilen tarihler arasında bu odada yeterli müsaitlik yok. Lütfen farklı tarih seçin."
                    });
                }

                // 3. Rezervasyon oluştur
                var rezervasyon = new Rezervasyon
                {
                    MusteriId = base.Musteri.Id,
                    OdaId = model.OdaId,
                    BaslangicTarihi = model.BaslangicTarihi,
                    BitisTarihi = model.BitisTarihi,
                    durum = "Beklemede",
                    MisafirBilgileri = model.MisafirBilgileri.Select(m => new MisafirBilgileri
                    {
                        Ad = m.Ad,
                        Soyad = m.Soyad,
                        TC = m.TC,
                        DogumTarihi = m.DogumTarihi,
                        Cinsiyet = m.Cinsiyet
                    }).ToList()
                };

                Tatildb.Rezervasyon.Add(rezervasyon);
                Tatildb.SaveChanges();

                TempData["SuccessMessage"] = "Rezervasyonunuz başarıyla kaydedildi!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                var hataMesaji = ex.InnerException?.Message ?? ex.Message;

                return Json(new
                {
                    isSuccess = false,
                    message = $"Rezervasyon kaydedilirken bir hata oluştu. Hata: {hataMesaji}"
                });
            }
        }

    }

}

