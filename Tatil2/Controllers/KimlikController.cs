using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tatil2.DBContext;
using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Controllers
{
    public class KimlikController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        public KimlikController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }
      
        private bool IsAuthenticatedUser()
        {
            var userSession = HttpContext.Session.GetString("LoginUser");
            return !string.IsNullOrEmpty(userSession);
        }
 
        private bool IsAdmin()
        {
            var userSession = HttpContext.Session.GetString("LoginUser");
            return !string.IsNullOrEmpty(userSession) && userSession == "admin";
        }

        public ActionResult Index()
        {

            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            var members = Tatildb.Musteri.ToList();
            return View(members);
        }

        public ActionResult Create()
        {

            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel registerUser)
        {

            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            if (ModelState.IsValid)
            {
                try
                {

                    var newUser = new Musteri()
                    {
                        Mail = registerUser.Mail,
                        Sifre = registerUser.Sifre,
                    };

                    Tatildb.Musteri.Add(newUser);
                    int changes = Tatildb.SaveChanges();


                    if (changes > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Veritabanına veri eklenirken bir hata oluştu.");
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", $"Hata: {ex.Message}");
                }
            }

            return View(registerUser);
        }


        public ActionResult Delete(int? id)
        {

            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            if (id == null)
            {
                return BadRequest();
            }

            var user = Tatildb.Musteri.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = Tatildb.Musteri.Find(id);
            if (user != null)
            {
                Tatildb.Musteri.Remove(user);
                Tatildb.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    }
}