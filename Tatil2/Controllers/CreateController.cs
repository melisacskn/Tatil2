using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Tatil2.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Tatil2.Controllers
{
    public class CreateController : Controller
    {
        private readonly TatilDBContext Tatildb;

        public CreateController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "123")
            {
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("OtelCreate");
            }
            else
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login");
        }

        public IActionResult OtelCreate()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login");
            }


            var sehirList = Tatildb.Sehir.ToList();
            ViewBag.Sehirler = sehirList;

            var tagKategoriList = Tatildb.TagKategori.Include(tk => tk.Tag).ToList(); // İlişkili Tag'lerle birlikte
            ViewBag.TagKategori = tagKategoriList;

            return View();
        }

        public IActionResult GetIlceler([FromQuery] int id)
        {
            var result = Tatildb.İlce.Where(x => x.SehirId == id).Select(x => new { x.Id, x.Ad }).ToList();
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OtelCreate(OtelCreateDTO otelCreate)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = Tatildb.Database.BeginTransaction())
                {
                    try
                    {
                        var otel = new Otel
                        {
                            Ad = otelCreate.Ad,
                            Aciklama = otelCreate.Aciklama,
                            Konum = otelCreate.Konum,
                            İlceId = otelCreate.İlceId
                        };

                        if (otelCreate.Poster != null)
                        {
                            string fileExtension = Path.GetExtension(otelCreate.Poster.FileName);
                            if (fileExtension.ToLower() != ".jpg" && fileExtension.ToLower() != ".png")
                            {
                                TempData["ErrorMessage"] = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz.";
                                //return View(otelCreate);
                                return Json(new {IsSuccess = false, Message= "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz." });
                            }
                            string posterAd = Path.GetFileNameWithoutExtension(otelCreate.Poster.FileName);
                            posterAd = Regex.Replace(posterAd, @"[^a-zA-Z0-9_-]", "");
                            posterAd = posterAd + fileExtension;

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", posterAd);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                otelCreate.Poster.CopyTo(stream);
                            }
                            otel.Poster = "/images/" + posterAd;
                        }

                        Tatildb.Otel.Add(otel);
                        Tatildb.SaveChanges();

                        var otelId = otel.Id;

                        if (otelCreate.Odalar != null)
                        {
                            foreach (var oda in otelCreate.Odalar)
                            {
                                var yeniOda = new Oda
                                {
                                    OdaAd = oda.OdaAd,
                                    OdaAciklama = oda.OdaAciklama,
                                    OdaFiyat = oda.OdaFiyat,
                                    KisiSayisi = oda.KisiSayisi,
                                    OdaStok = oda.OdaStok,
                                    OtelId = otel.Id,
                                };

                                if (oda.OdaFoto != null)
                                {
                                    string odaFotografExtension = Path.GetExtension(oda.OdaFoto.FileName);
                                    if (odaFotografExtension.ToLower() != ".jpg" && odaFotografExtension.ToLower() != ".png")
                                    {
                                        TempData["ErrorMessage"] = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz.";
                                        //return View(otelCreate);
                                        return Json(new { IsSuccess = false, Message = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz." });

                                    }

                                    string odaFotoAd = yeniOda.OdaAd + odaFotografExtension;
                                    var odaFotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", odaFotoAd);
                                    using (var stream = new FileStream(odaFotoPath, FileMode.Create))
                                    {
                                        oda.OdaFoto.CopyTo(stream);
                                    }
                                    yeniOda.OdaFoto = "/images/" + odaFotoAd;
                                }

                                Tatildb.Oda.Add(yeniOda);
                            }
                            Tatildb.SaveChanges();
                        }

                        if (otelCreate.SelectedTagId != null && otelCreate.SelectedTagId.Any())
                        {
                            Tatildb.OtelTag.AddRange(otelCreate.SelectedTagId.Select(id => new OtelTag() { OtelId = otel.Id, TagId = id }));
                            Tatildb.SaveChanges();
                        }

                        transaction.Commit();

                        TempData["SuccessMessage"] = "Otel başarıyla eklendi!";
                        return RedirectToAction("OtelCreate");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;

                        if (ex.InnerException != null)
                        {
                            TempData["ErrorMessage"] += " | Inner: " + ex.InnerException.Message;
                        }

                        return Json(new { IsSuccess = false, Message = TempData["ErrorMessage"] });
                    }
                }
            }

            //ViewBag.Sehirler = Tatildb.Sehir.ToList();
            return Json(new { IsSuccess = false, Message = "Zorunlu alanları doldurduğunuzdan emin olunuz." });
        }
    }
}