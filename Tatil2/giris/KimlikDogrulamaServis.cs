
using Tatil2.Interfaces;
using Tatil2.Models;
using Tatil2.Models.DTO;


namespace Tatil2.Giris
{
    public class KimlikDogrulamaServis : IAuthService
    {
        private readonly MisafirBilgileri _misafirBilgileri;

        public KimlikDogrulamaServis(MisafirBilgileri misafirBilgileri)
        {
            _misafirBilgileri = misafirBilgileri;
        }

        public Task<MisafirBilgileri> GetMisafirBilgileri(LoginViewModel request)
        {
            MisafirBilgileri response = new MisafirBilgileri();

            
            if (string.IsNullOrEmpty(request.Mail) || string.IsNullOrEmpty(request.Sifre))
            {
                throw new ArgumentNullException(nameof(request), "Ad veya şifre boş olamaz.");
            }

           
            if (request.Mail == "Melisa" && request.Sifre == "123456")
            {
                response.DogumTarihi = DateTime.UtcNow;
            }

            return Task.FromResult(response);
        }
    }
}
