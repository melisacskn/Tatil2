using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Tatil2.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Tatil2.Controllers
{
    public class HomeController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public HomeController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }



        public IActionResult Index()
        {
            string userJson = HttpContext.Session.GetString("login");

            if (string.IsNullOrEmpty(userJson))
            {
             
                return View();
            }

            var user = JsonConvert.DeserializeObject<Musteri>(userJson);


            var gecmisRezervasyonlar = Tatildb.Rezervasyon
            .Include(r => r.Oda)  
            .Where(r => r.MusteriId == user.Id)
            .OrderByDescending(r => r.BitisTarihi)
            .ToList();



            ViewBag.GecmisRezervasyonlar = gecmisRezervasyonlar;

            return View();
        }
    }
}
