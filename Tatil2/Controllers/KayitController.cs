using Microsoft.AspNetCore.Mvc;
using Tatil2.Models;
using Tatil2.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Tatil2.Controllers
{
    public class KayitController : Controller
    {
        private readonly TatilDBContext _context;


        public KayitController(TatilDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public IActionResult Index()
        {
            return View(new Musteri());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Musteri model)
        {
            if (model == null)
            {

                Console.WriteLine("Model null geldi!");
                return View();
            }

            if (ModelState.IsValid)
            {

                if (model.Telefon?.Length > 10)
                {
                    ModelState.AddModelError("Telefon", "Telefon numarası 10 karakterden fazla olamaz.");
                }

                if (model.TC?.Length > 11)
                {
                    ModelState.AddModelError("TC", "TC numarası 11 karakterden fazla olamaz.");
                }


                if (!string.IsNullOrEmpty(model.Mail) && _context.Musteri.Any(m => m.Mail == model.Mail))
                {
                    ModelState.AddModelError("Mail", "Bu e-posta adresi zaten kayıtlı.");
                }


                if (ModelState.IsValid)
                {
                    try
                    {

                        _context.Musteri.Add(model);


                        _context.SaveChanges();


                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Hata: {ex.Message}");
                        ModelState.AddModelError(" ", "Veritabanına eklerken bir hata oluştu.");
                    }
                }
            }


            return View(model);
        }
    }
}