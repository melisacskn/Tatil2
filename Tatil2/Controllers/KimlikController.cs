using Microsoft.AspNetCore.Mvc; 
using Tatil2.DBContext; 
using Tatil2.Models; 
using Tatil2.Models.DTO; 

namespace Tatil2.Controllers // KimlikController sınıfını Tatil2.Controllers ad alanı altında tanımla
{
    public class KimlikController : BaseController // KimlikController, BaseController'dan türetilir
    {
        private readonly TatilDBContext Tatildb; // DBContext sınıfını tanımla, veritabanı işlemleri için kullanılacak

        public KimlikController(TatilDBContext tatilDB) // Constructor, TatilDBContext sınıfı ile bağlıdır
        {
            Tatildb = tatilDB; // DBContext nesnesi atanır
        }

        private bool IsAuthenticatedUser() // Kullanıcının giriş yapıp yapmadığını kontrol eden fonksiyon
        {
            var userSession = HttpContext.Session.GetString("LoginUser"); // Oturumdaki LoginUser verisini al
            return !string.IsNullOrEmpty(userSession); // Eğer LoginUser bilgisi varsa kullanıcı giriş yapmış demektir
        }

        private bool IsAdmin() // Kullanıcının admin olup olmadığını kontrol eden fonksiyon
        {
            var userSession = HttpContext.Session.GetString("LoginUser"); // Oturumdaki LoginUser verisini al
            return !string.IsNullOrEmpty(userSession) && userSession == "admin"; // Eğer kullanıcı admin ise true döner
        }

        public ActionResult Index() // Index sayfasına yönlendirir (Müşterileri listeler)
        {
            if (!IsAuthenticatedUser()) return RedirectToAction("Home"); // Kullanıcı giriş yapmamışsa Home sayfasına yönlendir
            if (!IsAdmin()) return RedirectToAction("Home"); // Kullanıcı admin değilse Home sayfasına yönlendir

            var members = Tatildb.Musteri.ToList(); // Veritabanından tüm müşteri verilerini al
            return View(members); // Müşterilerle birlikte görünümü döndür
        }

        public ActionResult Create() // Yeni kullanıcı oluşturmak için formu gösterir
        {
            if (!IsAuthenticatedUser()) return RedirectToAction("Home"); // Kullanıcı giriş yapmamışsa Home sayfasına yönlendir
            if (!IsAdmin()) return RedirectToAction("Home"); // Kullanıcı admin değilse Home sayfasına yönlendir

            return View(); // Kullanıcıyı form sayfasına yönlendir
        }

        [HttpPost] // Bu metod, POST isteği ile tetiklenecektir
        [ValidateAntiForgeryToken] // CSRF koruması için anti-forgery token doğrulaması
        public ActionResult Create(RegisterViewModel registerUser) // Yeni kullanıcıyı kaydeden metod
        {
            if (!IsAuthenticatedUser()) return RedirectToAction("Home"); // Kullanıcı giriş yapmamışsa Home sayfasına yönlendir
            if (!IsAdmin()) return RedirectToAction("Home"); // Kullanıcı admin değilse Home sayfasına yönlendir

            if (ModelState.IsValid) // Eğer model doğrulama başarılı ise
            {
                try
                {
                    var newUser = new Musteri() // Yeni bir Musteri nesnesi oluştur
                    {
                        Mail = registerUser.Mail, // Kullanıcı mailini al
                        Sifre = registerUser.Sifre, // Kullanıcı şifresini al
                    };

                    Tatildb.Musteri.Add(newUser); // Yeni kullanıcıyı müşteri listesine ekle
                    int changes = Tatildb.SaveChanges(); // Değişiklikleri veritabanına kaydet

                    if (changes > 0) // Eğer veri kaydedildiyse
                    {
                        return RedirectToAction("Index"); // Kullanıcıyı Index sayfasına yönlendir
                    }
                    else
                    {
                        ModelState.AddModelError("", "Veritabanına veri eklenirken bir hata oluştu."); // Hata mesajı
                    }
                }
                catch (Exception ex) // Hata durumunda
                {
                    ModelState.AddModelError("", $"Hata: {ex.Message}"); // Hata mesajını model state'e ekle
                }
            }

            return View(registerUser); // Kullanıcı formu geri döndür, kullanıcıyı form ile tekrar göster
        }

        public ActionResult Delete(int? id) // Kullanıcı silme işlemi için ön izleme
        {
            if (!IsAuthenticatedUser()) return RedirectToAction("Home"); // Kullanıcı giriş yapmamışsa Home sayfasına yönlendir
            if (!IsAdmin()) return RedirectToAction("Home"); // Kullanıcı admin değilse Home sayfasına yönlendir

            if (id == null) // Eğer id parametresi null ise
            {
                return BadRequest(); // BadRequest döndür
            }

            var user = Tatildb.Musteri.Find(id); // Veritabanında kullanıcıyı id'ye göre ara
            if (user == null) // Eğer kullanıcı bulunamazsa
            {
                return NotFound(); // NotFound döndür
            }

            return View(user); // Kullanıcıyı silme sayfasında göster
        }

        [HttpPost, ActionName("Delete")] // Silme işlemi post ile yapılacak
        [ValidateAntiForgeryToken] // CSRF koruması için anti-forgery token doğrulaması
        public ActionResult DeleteConfirmed(int id) // Silme işlemi onaylandığında tetiklenir
        {
            var user = Tatildb.Musteri.Find(id); // Veritabanında kullanıcıyı id'ye göre ara
            if (user != null) // Eğer kullanıcı bulunmuşsa
            {
                Tatildb.Musteri.Remove(user); // Kullanıcıyı veritabanından sil
                Tatildb.SaveChanges(); // Değişiklikleri kaydet
                return RedirectToAction("Index"); // Index sayfasına yönlendir
            }

            return NotFound(); // Eğer kullanıcı bulunamazsa NotFound döndür
        }
    }
}
