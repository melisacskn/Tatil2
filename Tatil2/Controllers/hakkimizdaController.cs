using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;

namespace Tatil2.Controllers
{
     public class HakkimizdaController : Controller
        {
            
            public IActionResult Bilgi()
            {
                return View(); 
            }
        }
    }


