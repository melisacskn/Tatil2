using Microsoft.AspNetCore.Mvc;
using Tatil2.DBContext;
using Tatil2.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Tatil2.Controllers
{
    public class CreateController : BaseController
    {
        private readonly TatilDBContext Tatildb;

        // TatilDBContext sınıfı kullanılarak veri tabanı işlemleri yapılacak
        public CreateController(TatilDBContext tatilDB)
        {
            Tatildb = tatilDB;
        }

        // Login sayfasını kullanıcıya gösteren GET metodu
        public IActionResult Login()
        {
            return View();
        }

        // Login sayfasında kullanıcı adı ve şifreyi kontrol eden POST metodu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "123") // Şifre ve kullanıcı adı sabit
            {
                // Kullanıcı giriş yaptıysa oturumu başlatıyoruz
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("OtelCreate");
            }
            else
            {
                // Hatalı giriş yapılırsa, hata mesajı gösterilir
                TempData["ErrorMessage"] = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
        }

        // Logout işlemi, oturumu sonlandırır
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login");
        }

        // Otel ekleme sayfasına yönlendirir
        public IActionResult OtelCreate()
        {
            // Kullanıcı oturum açmamışsa Login sayfasına yönlendirilir
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login");
            }

            // Sehir listesini ve TagKategori listesini ViewBag ile view'e gönderir
            var sehirList = Tatildb.Sehir.ToList();
            ViewBag.Sehirler = sehirList;

            var tagKategoriList = Tatildb.TagKategori.Include(tk => tk.Tag).ToList();
            ViewBag.TagKategori = tagKategoriList;

            return View();
        }

        // İlçe verilerini alıp JSON olarak döndüren metod
        public IActionResult GetIlceler([FromQuery] int id)
        {
            // İlce verilerini SehirId'ye göre filtreler
            var result = Tatildb.İlce.Where(x => x.SehirId == id).Select(x => new { x.Id, x.Ad }).ToList();
            return Json(result); // JSON formatında döner
        }

        // Otel ekleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OtelCreate(OtelCreateDTO otelCreate)
        {
            if (ModelState.IsValid) // Eğer model geçerliyse işlem yapılır
            {
                using (var transaction = Tatildb.Database.BeginTransaction()) // Transaction başlatır, hata olursa geri alır
                {
                    try
                    {
                        // Otel nesnesi oluşturulur ve doldurulur
                        var otel = new Otel
                        {
                            Ad = otelCreate.Ad,
                            Aciklama = otelCreate.Aciklama,
                            Konum = otelCreate.Konum,
                            İlceId = otelCreate.İlceId
                        };

                        // Eğer poster dosyası varsa, dosyanın yüklenmesi yapılır
                        if (otelCreate.Poster != null)
                        {
                            string fileExtension = Path.GetExtension(otelCreate.Poster.FileName);
                            if (fileExtension.ToLower() != ".jpg" && fileExtension.ToLower() != ".png")
                            {
                                TempData["ErrorMessage"] = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz.";
                                return Json(new { IsSuccess = false, Message = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz." });
                            }
                            // Dosyanın adını düzenler ve kaydeder
                            string posterAd = Path.GetFileNameWithoutExtension(otelCreate.Poster.FileName);
                            posterAd = Regex.Replace(posterAd, @"[^a-zA-Z0-9_-]", "");
                            posterAd = posterAd + fileExtension;

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", posterAd);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                otelCreate.Poster.CopyTo(stream); // Dosya kaydedilir
                            }
                            otel.Poster = "/images/" + posterAd;
                        }

                        // Otel veri tabanına eklenir
                        Tatildb.Otel.Add(otel);
                        Tatildb.SaveChanges();

                        var otelId = otel.Id;

                        // Odalar varsa, her bir oda eklenir
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

                                // Oda fotoğrafı varsa yüklenir
                                if (oda.OdaFoto != null)
                                {
                                    string odaFotografExtension = Path.GetExtension(oda.OdaFoto.FileName);
                                    if (odaFotografExtension.ToLower() != ".jpg" && odaFotografExtension.ToLower() != ".png")
                                    {
                                        TempData["ErrorMessage"] = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz.";
                                        return Json(new { IsSuccess = false, Message = "Yalnızca .jpg veya .png dosyası yükleyebilirsiniz." });
                                    }

                                    string odaFotoAd = yeniOda.OdaAd + odaFotografExtension;
                                    var odaFotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", odaFotoAd);
                                    using (var stream = new FileStream(odaFotoPath, FileMode.Create))
                                    {
                                        oda.OdaFoto.CopyTo(stream); // Fotoğraf dosyası kaydedilir
                                    }
                                    yeniOda.OdaFoto = "/images/" + odaFotoAd;
                                }

                                Tatildb.Oda.Add(yeniOda); // Oda veritabanına eklenir
                            }
                            Tatildb.SaveChanges();
                        }

                        // Seçilen etiketler eklenir
                        if (otelCreate.SelectedTagId != null && otelCreate.SelectedTagId.Any())
                        {
                            Tatildb.OtelTag.AddRange(otelCreate.SelectedTagId.Select(id => new OtelTag() { OtelId = otel.Id, TagId = id }));
                            Tatildb.SaveChanges();
                        }

                        transaction.Commit(); // Transaction başarılı ise commit yapılır

                        TempData["SuccessMessage"] = "Otel başarıyla eklendi!";
                        return RedirectToAction("OtelCreate");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Hata olursa transaction geri alınır
                        TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;

                        if (ex.InnerException != null)
                        {
                            TempData["ErrorMessage"] += " | Inner: " + ex.InnerException.Message;
                        }

                        return Json(new { IsSuccess = false, Message = TempData["ErrorMessage"] });
                    }
                }
            }

            // Eğer model geçerli değilse hata mesajı döndürülür
            return Json(new { IsSuccess = false, Message = "Zorunlu alanları doldurduğunuzdan emin olunuz." });
        }
    }
}
