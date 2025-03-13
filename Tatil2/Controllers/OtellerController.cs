using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class OtellerController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public OtellerController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }
        public async Task<IActionResult> incele(int id)
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






