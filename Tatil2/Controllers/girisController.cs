using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Controllers
{
    public class girisController : Controller
    {
        private readonly TatilDBContext Tatildb;
        private readonly IConfiguration _configuration;

        public girisController(TatilDBContext tatilDB, IConfiguration configuration)
        {
            Tatildb = tatilDB;
            _configuration = configuration;
        }

        [HttpPost]
        public JsonResult SignIn([FromBody] LoginViewModel loginViewModel)
        {
            ModalLoginJsonResult result = new ModalLoginJsonResult();


            if (string.IsNullOrEmpty(loginViewModel.Mail) || string.IsNullOrEmpty(loginViewModel.Sifre))
            {
                result.HasError = true;
                result.Result = "E-posta ya da şifre boş geçilemez.";
            }
            else
            {

                Musteri user = Tatildb.Musteri.FirstOrDefault(x => x.Mail == loginViewModel.Mail && x.Sifre == loginViewModel.Sifre);

                if (user != null)
                {

                    result.HasError = false;
                    result.Result = "Giriş başarılı.";

                    Response.Cookies.Append("access_token", GenerateJwtToken(user), new CookieOptions()
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        //Secure = false
                    });

                    //user.Sifre = string.Empty;

                    //var userJson = JsonConvert.SerializeObject(user);
                    //HttpContext.Session.SetString("login", userJson);


                    return Json(new { HasError = false, Result = "Giriş başarılı.", redirectToUrl = Url.Action("Index", "Home") });
                }
                else
                {

                    result.HasError = true;
                    result.Result = "E-posta ya da şifre hatalı.";
                }
            }


            return Json(result);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        private string GenerateJwtToken(Musteri user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key")!);

            List<Claim> claims = [
                new Claim(ClaimTypes.Name, $"{user.Ad} {user.Soyad}"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Ad", user.Ad),
                new Claim("Soyad", user.Soyad),
                new Claim("Mail", user.Mail),
                new Claim("Telefon", user.Telefon),
                new Claim("TC", user.TC),
                new Claim("Cinsiyet", user.Cinsiyet.ToString()),
                ];
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration.GetValue<string>("Jwt:Issuer")!,
                Audience = _configuration.GetValue<string>("Jwt:Issuer")!,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
