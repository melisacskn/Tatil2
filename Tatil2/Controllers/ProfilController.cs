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

        // Constructor: TatilDBContext nesnesi ile controller'ı initialize eder
        public ProfilController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Profil sayfasına GET isteği ile erişildiğinde çalışacak metod
        // Kullanıcının geçmiş rezervasyonlarını ve profil bilgilerini getirir.
        [Authorize] // Yalnızca oturum açmış kullanıcılar erişebilir.
        [HttpGet] // HTTP GET isteği ile çalışacak.
        public IActionResult Profil()
        {
            // Kullanıcı bilgisini "Musteri" modelinden alır.
            var user = base.Musteri;

            // Kullanıcının geçmiş rezervasyonlarını alır (Oda bilgileri dahil).
            // Rezervasyonlar, son tarihine göre azalan sırayla sıralanır.
            var gecmisRezervasyonlar = Tatildb.Rezervasyon
                .Include(r => r.Oda) // Oda bilgilerini de dahil eder
                .Where(r => r.MusteriId == user.Id) // Kullanıcının rezervasyonları
                .OrderByDescending(r => r.BitisTarihi) // En son tarihli rezervasyon önce gelir
                .ToList();

            // Kullanıcının profil bilgilerini almak için bir Musteri nesnesi oluşturur.
            // Bu bilgiler profil sayfasında görüntülenecektir.
            var musteri = new Musteri
            {
                Ad = user.Ad, // Kullanıcının adı
                Soyad = user.Soyad, // Kullanıcının soyadı
                Telefon = user.Telefon, // Kullanıcının telefon numarası
            };

            // Profil sayfasına ilgili verilerle birlikte view'ı döndürür.
            return View();
        }
    }
}
