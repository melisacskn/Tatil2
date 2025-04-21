
using Tatil2.Interfaces;
using Tatil2.Models;
using Tatil2.Models.DTO;


namespace Tatil2.Giris
{
    // Kimlik doğrulama işlevini yerine getiren servis sınıfı
    public class KimlikDogrulamaServis : IAuthService
    {
        // Misafir bilgilerini tutan model
        private readonly MisafirBilgileri _misafirBilgileri;

        // Constructor - Misafir bilgilerini alır
        public KimlikDogrulamaServis(MisafirBilgileri misafirBilgileri)
        {
            _misafirBilgileri = misafirBilgileri;
        }

        // Kimlik doğrulama işlemi yapan metod
        public Task<MisafirBilgileri> GetMisafirBilgileri(LoginViewModel request)
        {
            MisafirBilgileri response = new MisafirBilgileri();

            // E-posta ve şifre kontrolü yapılır
            if (string.IsNullOrEmpty(request.Mail) || string.IsNullOrEmpty(request.Sifre))
            {
                // Eğer e-posta ya da şifre boşsa, hata fırlatılır
                throw new ArgumentNullException(nameof(request), "Ad veya şifre boş olamaz.");
            }

            // Sabit bir e-posta ve şifre ile doğrulama yapılır
            if (request.Mail == "Melisa" && request.Sifre == "123456")
            {
                // Doğru bilgiler girildiyse, MisafirBilgileri nesnesi güncellenir
                response.DogumTarihi = DateTime.UtcNow;  // Örnek olarak doğum tarihi atanıyor
            }
            else
            {
                // Eğer bilgiler yanlışsa, boş bir MisafirBilgileri nesnesi döndürülür
                response = null;
            }

            // Doğru veya yanlış kimlik bilgilerine göre MisafirBilgileri döndürülür
            return Task.FromResult(response);
        }
    }
}
