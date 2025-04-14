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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var gecmisRezervasyonlar = Tatildb.Rezervasyon
                    .Include(r => r.Oda)
                    .Where(r => r.MusteriId == userId)
                    .OrderByDescending(r => r.BitisTarihi)
                    .ToList();
            ViewBag.GecmisRezervasyonlar = gecmisRezervasyonlar;
            return View();
        }
    }
}
