using Tatil2.Models;

namespace Tatil2.Interfaces
{
    public interface IAuthService
    {
        Task<MisafirBilgileri> GetMisafirBilgileri(LoginViewModel request);
    }
}
