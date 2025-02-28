using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class KimlikController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public KimlikController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Kullanıcı doğrulaması
        private bool IsAuthenticatedUser()
        {
            var userSession = HttpContext.Session.GetString("LoginUser");
            return !string.IsNullOrEmpty(userSession);
        }

        // Admin kontrolü
        private bool IsAdmin()
        {
            var userSession = HttpContext.Session.GetString("LoginUser");
            return !string.IsNullOrEmpty(userSession) && userSession == "admin";
        }

        // Kullanıcılar listesini görüntüleme
        public ActionResult Index()
        {
            // Kullanıcı doğrulaması ve admin kontrolü yapılır
            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            // DbSet kullanarak kullanıcıları alıyoruz
            var members = Tatildb.Musteri.ToList();
            return View(members);
        }

        // Yeni kullanıcı oluşturma formu
        public ActionResult Create()
        {
            // Kullanıcı doğrulaması ve admin kontrolü yapılır
            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            return View();
        }

        // Kullanıcı ekleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel registerUser)
        {
            // Kullanıcı doğrulaması ve admin kontrolü yapılır
            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            // Model geçerliyse yeni kullanıcı ekle
            if (ModelState.IsValid)
            {
                try
                {
                    // Yeni kullanıcı oluşturuluyor
                    var newUser = new Musteri()
                    {
                        Mail = registerUser.Mail,
                        Sifre = registerUser.Sifre,
                    };

                    // DbSet kullanarak kullanıcı ekliyoruz
                    Tatildb.Musteri.Add(newUser);
                    int changes = Tatildb.SaveChanges();

                    // Değişiklikler kaydedildiyse yönlendir
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
                    // Hata mesajını loglama veya görüntüleme
                    ModelState.AddModelError("", $"Hata: {ex.Message}");
                }
            }

            return View(registerUser);
        }

        // Kullanıcı silme işlemi
        public ActionResult Delete(int? id)
        {
            // Kullanıcı doğrulaması ve admin kontrolü yapılır
            if (!IsAuthenticatedUser()) return RedirectToAction("Home");
            if (!IsAdmin()) return RedirectToAction("Home");

            if (id == null)
            {
                return BadRequest();
            }

            // DbSet kullanarak kullanıcıyı buluyoruz
            var user = Tatildb.Musteri.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Silme işlemi sonrası
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