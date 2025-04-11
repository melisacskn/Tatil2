using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class ProfilController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public ProfilController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // GET: Profil
        [HttpGet]
        public IActionResult Profil()
        {
            // Kullanıcıyı session'dan al
            string userJson = HttpContext.Session.GetString("login");
            if (string.IsNullOrEmpty(userJson))
            {
                TempData["ErrorMessage"] = "Lütfen önce giriş yapın.";
                return RedirectToAction("Index", "Giris");
            }

            // Kullanıcı verilerini deserialize et
            var user = JsonConvert.DeserializeObject<Musteri>(userJson);

            // Geçmiş rezervasyonları al
            var gecmisRezervasyonlar = Tatildb.Rezervasyon
                .Include(r => r.Oda)
                .Where(r => r.MusteriId == user.Id)
                .OrderByDescending(r => r.BitisTarihi)
                .ToList();

            // Profil bilgilerini model olarak view'a gönder
            var musteri = new Musteri
            {
                Ad = user.Ad,
                Soyad = user.Soyad,
                Telefon = user.Telefon,
               
            };

            return View();
        }

    }
}
