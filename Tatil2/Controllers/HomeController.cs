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
    public class HomeController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        public HomeController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }


        [Authorize]
        public IActionResult Index()
        {

            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userId = base.Musteri.Id;

            var gecmisRezervasyonlar = Tatildb.Rezervasyon
                    .Where(r => r.MusteriId == userId)
                    .Include(r => r.Oda)
                    .OrderByDescending(r => r.BitisTarihi)
                    .ToList();
            ViewBag.GecmisRezervasyonlar = gecmisRezervasyonlar;
            return View();
        }
    }
}
