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

        public async Task<IActionResult> Index()
        {
            // Bu kısımda Include metodunu kullanıyoruz
            var otel = await Tatildb.Otel.Include(o => o.İlce).ToListAsync();
            return View(otel);
        }

        // Otel ekleme formu
        public IActionResult Create()
            {
                ViewData["İlceId"] = new SelectList(Tatildb.İlce, "Id", "Name");
                return View();
            }

           
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Otel otel)
            {
                if (ModelState.IsValid)
                {
                    Tatildb.Add(otel);
                    await Tatildb.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["İlceId"] = new SelectList(Tatildb.İlce, "Id", "Name", otel.İlceId);
                return View(otel);
            }
        }
    }








