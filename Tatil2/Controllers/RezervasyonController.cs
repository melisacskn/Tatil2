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
        public IActionResult Rezervasyon([FromQuery] RezervasyonDTO rezervasyon)
        {
            var yorumlar = Tatildb.Yorum
                .Where(y => y.OdaId == rezervasyon.OdaId)
                .Include(y => y.Musteri)
                .ToList();

            ViewBag.Yorumlar = yorumlar;

            return View(rezervasyon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumYap(int odaId, string yorumMetni, int puan)
        {
            if (Musteri == null) 
                return RedirectToAction("SignIn", "Giris");

            var oda = await Tatildb.Oda.FirstOrDefaultAsync(o => o.Id == odaId);
            if (oda == null)
                return NotFound();

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

            Tatildb.Yorum.Add(yorum);
            await Tatildb.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yorumunuz başarıyla kaydedildi.";
            return RedirectToAction("Rezervasyon", new RezervasyonDTO
            {
                OdaId = oda.Id,
                BaslangicTarihi = DateTime.Now, 
                BitisTarihi = DateTime.Now.AddDays(1)
            });
        }





        //[HttpPost]
        //public IActionResult Rezervasyon(RezervasyonCreateDTO model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new { Message = "Giriş bilgilerini kontrol ediniz" });
        //    }

        //    try
        //    {
        //        string userJson = HttpContext.Session.GetString("login");
        //        if (string.IsNullOrWhiteSpace(userJson))
        //            return Json(new { Message = "Oturum süresi doldu, lütfen tekrar giriş yapınız." });

        //        var user = JsonConvert.DeserializeObject<Musteri>(userJson);

        //        Rezervasyon rezervasyon = new Rezervasyon
        //        {
        //            MusteriId = user.Id,
        //            OdaId = model.OdaId,
        //            BaslangicTarihi = model.BaslangicTarihi,
        //            BitisTarihi = model.BitisTarihi
        //        };

        //        Tatildb.Rezervasyon.Add(rezervasyon);
        //        Tatildb.SaveChanges();

        //        TempData["SuccessMessage"] = "Rezervasyonunuz başarıyla tamamlandı!";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Rezervasyon hatası: {ex.Message}");
        //        return Json(new { Message = "Rezervasyon işlemi sırasında bir hata oluştu." });
        //    }
        //}


        [HttpGet]
        public IActionResult Tamamla(int odaId, DateTime baslangic, DateTime bitis, int kisiSayisi)
        {
            var oda = Tatildb.Oda.Include(o => o.Otel).FirstOrDefault(o => o.Id == odaId);
            if (oda == null) return NotFound();

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

            for (int i = 0; i < kisiSayisi; i++)
            {
                viewModel.MisafirBilgileri.Add(new MisafirBilgileri());
            }

            return View(viewModel);
        }
       


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

                var rezervasyon = new Rezervasyon
                {
                    MusteriId = base.Musteri.Id,
                    OdaId = model.OdaId,
                    BaslangicTarihi = model.BaslangicTarihi,
                    BitisTarihi = model.BitisTarihi,
                };

                //Tatildb.SaveChanges();

                //TODO: Misafir bilgileri sayısı ile kişi sayısı eşleşmeli
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

                rezervasyon.MisafirBilgileri.AddRange(misafirBilgileri);
                Tatildb.Rezervasyon.Add(rezervasyon);
                Tatildb.SaveChanges();

                TempData["SuccessMessage"] = "Rezervasyonunuz başarıyla kaydedildi!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Rezervasyon kaydedilirken bir hata oluştu. Hata: {ex.Message}";
                // Hata ayrıntılarını konsola yazdırarak daha fazla bilgi alabilirsiniz.
                Console.WriteLine(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = "Rezervasyon kaydedilirken bir hata oluştu. Lütfen tekrar deneyin."
                });
            }

        }

    }
}

