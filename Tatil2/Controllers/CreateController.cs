using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Tatil2.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Tatil2.Controllers
{
    public class CreateController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public CreateController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        public IActionResult OtelCreate()
        {
            ViewBag.Ilceler = Tatildb.İlce.ToList();  // İlçeleri ViewBag ile gönderiyoruz
            ViewBag.Oteller = Tatildb.Otel.ToList();  // Eklenen otelleri de gönderiyoruz
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OtelCreate(OtelCreateDTO otelCreate)
        {
            if (ModelState.IsValid)
            {
                var otel = new Otel();
                otel.Ad = otelCreate.Ad;
                otel.Aciklama = otelCreate.Aciklama;
                otel.Konum = otelCreate.Konum;
                otel.İlceId = otelCreate.İlceId;

                if (otelCreate.Poster != null)
                {
                    string fileextension = Path.GetExtension(otelCreate.Poster.FileName);
                    string posterAd = otel.Ad + fileextension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", posterAd);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        otelCreate.Poster.CopyTo(stream);
                    }
                    otel.Poster = "/images/" + posterAd;
                }


                Tatildb.Otel.Add(otel);
                Tatildb.SaveChanges();

                TempData["SuccessMessage"] = "Otel başarıyla eklendi!";


                return RedirectToAction("OtelCreate");
            }



            ViewBag.Ilceler = Tatildb.İlce.ToList();
            ViewBag.Oteller = Tatildb.Otel.ToList();
            return View(otelCreate);
        }

    }

}

