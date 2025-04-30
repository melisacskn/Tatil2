using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class AdminController:BaseController
    {
        private readonly TatilDBContext Tatildb;

     
        public AdminController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }
        public IActionResult Admin()
        {
            var son3 = Tatildb.Rezervasyon
                               .OrderByDescending(r => r.BaslangicTarihi)
                               .Take(3)
                               .ToList();

            // Null kontrolü eklemek:
            if (son3 == null || !son3.Any())
            {
                // Boş veri durumu
                ViewBag.ErrorMessage = "Hiç rezervasyon bulunamadı.";
                return View(new List<Rezervasyon>());  
            }

            return View(son3);  
        }

    }
}
