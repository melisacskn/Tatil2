using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tatil2.DBContext;
using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Controllers
{
    public class AdminController : BaseController
    {
        private readonly TatilDBContext Tatildb;


        public AdminController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }
        public IActionResult Admin()
        {
            var rezervasyon = Tatildb.Rezervasyon
                                        .Include(r => r.Musteri)
                                        .Include(r => r.Oda)
                                        .OrderByDescending(r => r.BaslangicTarihi)
                                        .Take(3)
                                        .ToList();

            if (rezervasyon == null || !rezervasyon.Any())
            {
                ViewBag.ErrorMessage = "Hiç rezervasyon bulunamadı.";
                rezervasyon = new List<Rezervasyon>();
            }

            var yorum = Tatildb.Yorum
                                  .Include(y => y.Musteri)
                                  .Include(y => y.Oda)
                                  .Include(y => y.Otel)
                                  .OrderByDescending(y => y.Id)
                                  .Take(5)
                                  .ToList();

            var model = new AdminPanelViewModel
            {
                Rezervasyon = rezervasyon,
                Yorum = yorum
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult RezervasyonOnayla(int id)
        {
            var rezervasyon = Tatildb.Rezervasyon.FirstOrDefault(r => r.Id == id);
            if (rezervasyon == null)
                return NotFound();

            rezervasyon.durum = "Onaylandı";
            Tatildb.SaveChanges();

            return RedirectToAction("Detay", new { id = id });
        }

        [HttpPost]
        public IActionResult RezervasyonReddet(int id)
        {
            var rezervasyon = Tatildb.Rezervasyon.FirstOrDefault(r => r.Id == id);
            if (rezervasyon == null)
                return NotFound();

            rezervasyon.durum = "Reddedildi";
            Tatildb.SaveChanges();

            return RedirectToAction("Detay", new { id = id });
        }



        public IActionResult Detay(int id)
        {
            var rezervasyon = Tatildb.Rezervasyon
                .Include(r => r.Musteri)
                .Include(r => r.MisafirBilgileri)
                .Include(r => r.Oda)
                .FirstOrDefault(r => r.Id == id);

            if (rezervasyon == null)
            {
                return NotFound();
            }

            return View(rezervasyon);
        }

        public IActionResult TumRezervasyonlar()
        {
            var tumRezervasyonlar = Tatildb.Rezervasyon
                .Include(r => r.Musteri)
                .OrderByDescending(r => r.BaslangicTarihi)
                .ToList();

            return View(tumRezervasyonlar);
        }
        public IActionResult YorumDetay(int id)
        {
            var Yorum = Tatildb.Yorum
                .Include(r => r.Musteri)
                .Include(r => r.Otel)
                .Include(r => r.Oda)
                .FirstOrDefault(r => r.Id == id);

            if (Yorum == null)
            {
                return NotFound();
            }

            return View(Yorum);
        }
        public IActionResult TumYorumlar()
        {
            var TumYorumlar = Tatildb.Yorum
                .Include(r => r.Musteri)
                .Include(r => r.Oda)
                .Include(r => r.Otel)
                .OrderByDescending(r => r.MusteriId)
                .ToList();

            return View(TumYorumlar);
        }
        [HttpPost]
        public IActionResult RezervasyonGuncelle(Rezervasyon model)
        {
            if (model.BaslangicTarihi < new DateTime(1753, 1, 1) || model.BitisTarihi < new DateTime(1753, 1, 1))
            {
                ModelState.AddModelError("", "Lütfen geçerli bir tarih aralığı giriniz.");
                return View("Detay", model);
            }
           

            var rezervasyon = Tatildb.Rezervasyon
                .Include(r => r.Musteri)
                .FirstOrDefault(r => r.Id == model.Id);

            if (rezervasyon != null)
            {
                rezervasyon.Musteri.Ad = model.Musteri.Ad;
                rezervasyon.Musteri.Soyad = model.Musteri.Soyad;
                rezervasyon.Musteri.Telefon = model.Musteri.Telefon;
                rezervasyon.Musteri.Mail = model.Musteri.Mail;
                rezervasyon.BaslangicTarihi = model.BaslangicTarihi;
                rezervasyon.BitisTarihi = model.BitisTarihi;



                Tatildb.SaveChanges();
            }

            return RedirectToAction("Detay", new { id = model.Id });
        }
        [HttpPost]

        public IActionResult MisafirGuncelle(MisafirBilgileri misafir)
        {
            var mevcutMisafir = Tatildb.MisafirBilgileri.FirstOrDefault(m => m.Id == misafir.Id);

            if (mevcutMisafir != null)
            {
                mevcutMisafir.Ad = misafir.Ad;
                mevcutMisafir.Soyad = misafir.Soyad;
                mevcutMisafir.TC = misafir.TC;
                mevcutMisafir.DogumTarihi = misafir.DogumTarihi;

                Tatildb.SaveChanges();
            }

            return RedirectToAction("Detay", new { id = mevcutMisafir.RezervasyonId });
        }
        [HttpPost]
        public IActionResult YorumSilOnay(int yorumId, List<string> nedenler, string digerSebep)
        {
            // Silme nedenlerini birleştir
            string tumNedenler = string.Join(", ", nedenler ?? new List<string>());
            if (!string.IsNullOrWhiteSpace(digerSebep))
            {
                tumNedenler += $" | Diğer Açıklama: {digerSebep}";
            }

            // Loglama veya denetim amaçlı
            Console.WriteLine($"Yorum ID: {yorumId} siliniyor. Nedenler: {tumNedenler}");

            // Yorum veritabanından siliniyor
            var yorum = Tatildb.Yorum.FirstOrDefault(y => y.Id == yorumId);
            if (yorum != null)
            {
                Tatildb.Yorum.Remove(yorum);
                Tatildb.SaveChanges();
                TempData["SilmeMesaji"] = "Yorum başarıyla silindi.";
            }
            else
            {
                TempData["SilmeMesaji"] = "Yorum bulunamadı.";
            }

            // Geri yönlendirme
            return RedirectToAction("Admin");
        }

    }
}
