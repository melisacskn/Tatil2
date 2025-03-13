using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class OtelgosterController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public OtelgosterController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        public async Task<IActionResult> Index()
        {
            var oteller = await Tatildb.Otel
                 .Include(o => o.İlce)
                 .Include(o => o.Odalar)
                 .ToListAsync();

            if (oteller == null || !oteller.Any())
            {
                // Veriler boş veya null geldiğinde, uygun bir hata mesajı gösterebilirsiniz
                return NotFound("Otel bulunamadı.");
            }

            return View(oteller);

        }

        public async Task<IActionResult> Incele(int id)
        {
            var otel = await Tatildb.Otel
                .Include(o => o.İlce)          
                .Include(o => o.Odalar)        
                .FirstOrDefaultAsync(o => o.Id == id); 
     
            if (otel == null)
            {
                return NotFound();
            }

            return View(otel); 
        }
    }
}
