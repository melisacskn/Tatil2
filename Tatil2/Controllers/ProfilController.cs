using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.Models;
using Microsoft.AspNetCore.Authorization;

namespace Tatil2.Controllers
{
    public class ProfilController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        public ProfilController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // GET: Profil
        [Authorize]
        [HttpGet]
        public IActionResult Profil()
        {
            var user = base.Musteri;

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
