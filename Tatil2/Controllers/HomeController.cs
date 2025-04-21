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
    // HomeController, BaseController'dan t�remektedir
    public class HomeController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        // TatilDBContext'i denet�iye enjekte etmek i�in constructor
        public HomeController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Index sayfas� i�in Action metodu, kullan�c�dan yetkilendirme gerektirir
        [Authorize]
        public IActionResult Index()
        {
            // Mevcut oturum a�m�� kullan�c�n�n ID'sini base s�n�f�ndan (BaseController) al�yoruz
            var userId = base.Musteri.Id;

            // Kullan�c�n�n ge�mi�teki rezervasyonlar�n� veritaban�ndan al�yoruz
            // Oda (Room) bilgilerini de i�eriyor ve biti� tarihine g�re azalan �ekilde s�ral�yoruz
            var gecmisRezervasyonlar = Tatildb.Rezervasyon
                    .Where(r => r.MusteriId == userId) // Kullan�c� ID'sine g�re filtreleme
                    .Include(r => r.Oda)               // �lgili 'Oda' (Room) verilerini de dahil etme
                    .OrderByDescending(r => r.BitisTarihi) // Biti� tarihine g�re azalan s�ralama
                    .ToList();

            // Elde edilen ge�mi� rezervasyonlar� View'a g�ndermek i�in ViewBag'e at�yoruz
            ViewBag.GecmisRezervasyonlar = gecmisRezervasyonlar;

            // G�r�n�m� (view) d�nd�r�yoruz
            return View();
        }
    }
}
