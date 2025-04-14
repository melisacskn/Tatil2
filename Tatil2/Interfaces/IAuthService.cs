using Tatil2.Models;
using Tatil2.Models.DTO;

namespace Tatil2.Interfaces
{
    public interface IAuthService
    {
        Task<MisafirBilgileri> GetMisafirBilgileri(LoginViewModel request);
    }
}
