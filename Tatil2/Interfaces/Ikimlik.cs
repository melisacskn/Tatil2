using System.Threading.Tasks;
using Tatil2.Models;

namespace Tatil2.Interfaces
{
    public interface Ikimlik
    {
        public Task<Musteri> LoginUserAsync(Musteri request);
    }
}
