using Microsoft.AspNetCore.Mvc;
using Tatil2.Models;
using Tatil2.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Tatil2.Controllers
{
    public class KayitController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        // Constructor (Yapıcı metod) ile TatilDBContext'i enjekte ediyoruz.
        public KayitController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // GET metoduyla kayıt formunun gösterilmesi
        public IActionResult Index()
        {
            return View(new Musteri());
        }

        // POST metoduyla formun işlenmesi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Musteri model)
        {
            // Eğer model null ise, hata mesajı yazdırılıyor
            if (model == null)
            {
                Console.WriteLine("Model null geldi!");
                return View();
            }

            // ModelState geçerli mi kontrol ediliyor
            if (ModelState.IsValid)
            {
                // Telefon numarasının 10 karakteri aşmadığından emin olunuyor
                if (model.Telefon?.Length > 10)
                {
                    ModelState.AddModelError("Telefon", "Telefon numarası 10 karakterden fazla olamaz.");
                }

                // TC numarasının 11 karakteri aşmadığından emin olunuyor
                if (model.TC?.Length > 11)
                {
                    ModelState.AddModelError("TC", "TC numarası 11 karakterden fazla olamaz.");
                }

                // Aynı e-posta adresinin daha önce kayıtlı olup olmadığı kontrol ediliyor
                if (!string.IsNullOrEmpty(model.Mail) && Tatildb.Musteri.Any(m => m.Mail == model.Mail))
                {
                    ModelState.AddModelError("Mail", "Bu e-posta adresi zaten kayıtlı.");
                }

                // ModelState geçerli ise veritabanına ekleniyor
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Yeni müşteri ekleniyor
                        Tatildb.Musteri.Add(model);

                        // Değişiklikler kaydediliyor
                        Tatildb.SaveChanges();

                        // Başarılı kayıt sonrası anasayfaya yönlendirme
                        return RedirectToAction("Index", "Oteller");
                    }
                    catch (Exception ex)
                    {
                        // Hata mesajı yazdırılıyor
                        Console.WriteLine($"Hata: {ex.Message}");
                        ModelState.AddModelError(" ", "Veritabanına eklerken bir hata oluştu.");
                    }
                }
            }

            // Geçersiz model durumunda tekrar form gösteriliyor
            return View(model);
        }
    }
}
