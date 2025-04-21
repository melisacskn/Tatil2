using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class BaseController : Controller
    {
        private Musteri musteri;  // 'Musteri' sınıfından bir nesne tanımlanıyor.

        // Musteri özelliği, kullanıcı bilgilerini her çağrıldığında yüklemek için kullanılıyor.
        public Musteri Musteri
        {
            get
            {
                if (musteri == null)  // Eğer 'musteri' nesnesi henüz oluşturulmamışsa, 'SetMusteri()' metodu çağrılır.
                {
                    SetMusteri();
                }
                return musteri;  // Kullanıcı bilgilerini içeren 'Musteri' nesnesini döndürür.
            }
        }

        // Kullanıcı bilgilerini 'Claims' üzerinden alarak 'Musteri' nesnesine atama yapan metod.
        private Musteri SetMusteri()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;  // Kullanıcının kimlik bilgilerini alır.

            if (claimsIdentity == null)  // Eğer kimlik bilgileri mevcut değilse, null döndürülür.
                return null;

            var claims = claimsIdentity.Claims;  // Kullanıcıya ait tüm 'Claim' verilerini alır.

            // 'Musteri' nesnesi, 'Claims' üzerinden gelen verilere göre doldurulur.
            musteri = new Musteri
            {
                Id = int.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),  // Kullanıcının ID'si alınır.
                Ad = claims.First(c => c.Type == "Ad").Value,  // Kullanıcının adı alınır.
                Soyad = claims.First(c => c.Type == "Soyad").Value,  // Kullanıcının soyadı alınır.
                Mail = claims.First(c => c.Type == "Mail").Value,  // Kullanıcının e-posta adresi alınır.
                Telefon = claims.First(c => c.Type == "Telefon").Value,  // Kullanıcının telefon numarası alınır.
                TC = claims.First(c => c.Type == "TC").Value,  // Kullanıcının TC kimlik numarası alınır.
                Cinsiyet = bool.Parse(claims.First(c => c.Type == "Cinsiyet").Value),  // Kullanıcının cinsiyeti alınır.
                IsAdmin = claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin")  // Kullanıcının admin olup olmadığı kontrol edilir.
            };

            return musteri;  // Doldurulan 'Musteri' nesnesi geri döndürülür.
        }
    }
}
