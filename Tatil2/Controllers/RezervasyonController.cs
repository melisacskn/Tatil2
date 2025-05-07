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
                // Rezervasyon oluşturuluyor
                var rezervasyon = new Rezervasyon
                {
                    MusteriId = base.Musteri.Id,
                    OdaId = model.OdaId,
                    BaslangicTarihi = model.BaslangicTarihi,
                    BitisTarihi = model.BitisTarihi,
                };

                // Misafir bilgileri ekleniyor
                var misafirBilgileri = new List<MisafirBilgileri>();

                foreach (var misafir in model.MisafirBilgileri)
                {
                    var misafirBilgi = new MisafirBilgileri
                    {
                        Ad = misafir.Ad,
                        Soyad = misafir.Soyad,
                        TC = misafir.TC,
                        DogumTarihi = misafir.DogumTarihi,
                        Cinsiyet = misafir.Cinsiyet,
                    };
                    misafirBilgileri.Add(misafirBilgi);
                }

                // Misafir bilgileri rezervasyona ekleniyor
                rezervasyon.MisafirBilgileri.AddRange(misafirBilgileri);
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

