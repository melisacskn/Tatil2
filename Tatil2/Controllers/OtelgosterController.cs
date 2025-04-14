using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tatil2.DBContext;
using Tatil2.Models;

namespace Tatil2.Controllers
{
    public class OtelgosterController : BaseController
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
                return NotFound("Otel bulunamadı.");
            }

            return View(oteller);

        }

    }
}