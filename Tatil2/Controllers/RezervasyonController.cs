using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public RezervasyonController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }
        public IActionResult Rezerve()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RezervasyonYap(Rezervasyon rezervasyon)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await Tatildb.Database.BeginTransactionAsync())

                    try
                    {
                        {
                            // Kart Bilgilerini ekliyoruz
                            var kartBilgisi = new KartBilgisi
                            {
                                Iban = rezervasyon.KartBilgisi.Iban,
                                KartTarih = rezervasyon.KartBilgisi.KartTarih,
                                Cvv = rezervasyon.KartBilgisi.Cvv
                            };

                            // Kart bilgilerini veritabanına ekliyoruz
                            Tatildb.KartBilgisi.Add(kartBilgisi);
                            await Tatildb.SaveChangesAsync(); // Kart bilgilerini kaydediyoruz

                            // Rezervasyonu ekliyoruz
                            rezervasyon.KartBilgisiId = kartBilgisi.Id;  // KartBilgisiId'yi ayarlıyoruz
                            Tatildb.Rezervasyon.Add(rezervasyon);
                            await Tatildb.SaveChangesAsync();  // Rezervasyonu kaydediyoruz
                            transaction.Commit();
                            return RedirectToAction("Index", "Home");  // Başka bir sayfaya yönlendirebiliriz
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Json(new { message = ex.Message });
                    }


            }

            return View(rezervasyon);
        }
    }
}
