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
                .OrderByDescending(r => r.MusteriId)
                .ToList();

            return View(TumYorumlar);
        }
    }
}
