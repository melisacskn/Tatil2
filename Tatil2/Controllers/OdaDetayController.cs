using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class OdaDetayController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        public OdaDetayController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        public IActionResult OdaDetay(int odaId)
        {
            var oda = Tatildb.Oda.FirstOrDefault(o => o.Id == odaId);

            if (oda == null)
                return NotFound();

            // Odanın yorumlarını alıyoruz ve Yorum yapan müşteri bilgilerini de dahil ediyoruz.
            var yorumlar = Tatildb.Yorum
                .Where(y => y.OdaId == odaId)
                .Include(y => y.Musteri)  // Yorum yapan kişinin bilgilerini alıyoruz
                .ToList();

            // Verileri ViewBag'e ekliyoruz.
            ViewBag.Oda = oda;
            ViewBag.Yorumlar = yorumlar;

            return View();  // OdaDetay sayfasına yönlendiriyoruz.
        }
    }
}
