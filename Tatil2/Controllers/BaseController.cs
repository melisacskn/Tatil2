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

       
        private Musteri SetMusteri()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity; 

            if (claimsIdentity == null)  
                return null;

            var claims = claimsIdentity.Claims;  

          
            musteri = new Musteri
            {
                Id = int.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),  
                Ad = claims.First(c => c.Type == "Ad").Value, 
                Soyad = claims.First(c => c.Type == "Soyad").Value, 
                Mail = claims.First(c => c.Type == "Mail").Value,  
                Telefon = claims.First(c => c.Type == "Telefon").Value,  
                TC = claims.First(c => c.Type == "TC").Value, 
                Cinsiyet = bool.Parse(claims.First(c => c.Type == "Cinsiyet").Value),  
                IsAdmin = claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin")  
            };

            return musteri;  
        }
    }
}
