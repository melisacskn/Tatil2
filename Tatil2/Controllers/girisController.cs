using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class girisController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public girisController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        [HttpPost]
        public JsonResult SignIn([FromBody] LoginViewModel loginViewModel)
        {
            ModalLoginJsonResult result = new ModalLoginJsonResult();

            // Eğer e-posta veya şifre boşsa
            if (string.IsNullOrEmpty(loginViewModel.Mail) || string.IsNullOrEmpty(loginViewModel.Sifre))
            {
                result.HasError = true;
                result.Result = "E-posta ya da şifre boş geçilemez.";
            }
            else
            {
                // Kullanıcıyı veritabanında bulma
                Musteri user = Tatildb.Musteri.FirstOrDefault(x => x.Mail == loginViewModel.Mail && x.Sifre == loginViewModel.Sifre);

                if (user != null)
                {
                    // Giriş başarılı
                    result.HasError = false;
                    result.Result = "Giriş başarılı.";

                    // Şifreyi temizle
                    user.Sifre = string.Empty;

                    // Kullanıcı bilgilerini session'a kaydet
                    var userJson = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("login", userJson);

                    // Yönlendirme URL'si ile dönüyoruz (Home/Index sayfasına yönlendiriyoruz)
                    return Json(new { HasError = false, Result = "Giriş başarılı.", redirectToUrl = Url.Action("Index", "Home") });
                }
                else
                {
                    // Hatalı giriş
                    result.HasError = true;
                    result.Result = "E-posta ya da şifre hatalı.";
                }
            }

            // Hata mesajı döndürüyoruz
            return Json(result);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SignUp(string register_Mail, string register_password)
        {
            ModalLoginJsonResult result = new ModalLoginJsonResult();

            register_Mail = register_Mail.Trim();
            register_password = register_password.Trim();

            if (string.IsNullOrEmpty(register_Mail) || string.IsNullOrEmpty(register_password))
            {
                result.HasError = true;
                result.Result = "Lütfen tüm alanları doldurunuz.";
            }
            else
            {
                var existingUser = Tatildb.Musteri.FirstOrDefault(x => x.Mail == register_Mail);

                if (existingUser != null)
                {
                    result.HasError = true;
                    result.Result = "Bu e-posta adresi zaten kullanımda.";
                }
                else
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(register_password);

                    var newUser = new Musteri()
                    {
                        Mail = register_Mail,
                        Sifre = hashedPassword
                    };

                    Tatildb.Musteri.Add(newUser);

                    if (Tatildb.SaveChanges() > 0)
                    {
                        result.HasError = false;
                        result.Result = "Hesap başarıyla oluşturuldu.";

                        var userJson = JsonConvert.SerializeObject(newUser);
                        HttpContext.Session.SetString("LoginUser", userJson);
                    }
                    else
                    {
                        result.HasError = true;
                        result.Result = "Bir hata oluştu. Lütfen tekrar deneyin.";
                    }
                }
            }

            return Json(result);
        }

        public ActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
