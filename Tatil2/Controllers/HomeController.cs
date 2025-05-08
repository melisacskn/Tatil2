using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Tatil2.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Tatil2.Controllers
{
    // HomeController, BaseController'dan türemektedir
    public class HomeController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        // TatilDBContext'i denetçiye enjekte etmek için constructor
        public HomeController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Index sayfasý için Action metodu, kullanýcýdan yetkilendirme gerektirir
        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Admin", "Admin");
            }
            // Mevcut oturum açmýþ kullanýcýnýn ID'sini base sýnýfýndan (BaseController) alýyoruz
            var userId = base.Musteri.Id;

            // Kullanýcýnýn geçmiþteki rezervasyonlarýný veritabanýndan alýyoruz
            // Oda (Room) bilgilerini de içeriyor ve bitiþ tarihine göre azalan þekilde sýralýyoruz
            var gecmisRezervasyonlar = Tatildb.Rezervasyon
                    .Where(r => r.MusteriId == userId)
                    .Include(r => r.Oda)               
                    .OrderByDescending(r => r.BitisTarihi) 
                    .ToList();

            // Elde edilen geçmiþ rezervasyonlarý View'a göndermek için ViewBag'e atýyoruz
            ViewBag.GecmisRezervasyonlar = gecmisRezervasyonlar;

            // Görünümü (view) döndürüyoruz
            return View();
        }
    }
}
