using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Interfaces
{
    // Kimlik doğrulama servisinin sağlanması gereken metotları tanımlar
    public interface IAuthService
    {
        // Kullanıcı giriş bilgilerini alır ve karşılık gelen MisafirBilgileri nesnesini döndürür
        Task<MisafirBilgileri> GetMisafirBilgileri(LoginViewModel request);
    }
}
