using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tatil2.DBContext; // Konsol loglama i�in gerekli namespace

var builder = WebApplication.CreateBuilder(args);

// Logging yap�land�rmas�n� ekleyin
builder.Logging.ClearProviders();
builder.Logging.AddConsole();  // Konsola loglama ekleyin

// DbContext'inizi ekleyin
builder.Services.AddDbContext<TatilDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Di�er servis eklemeleri...
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Tatil2.Session"; // �erez ad�
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.IsEssential = true; // �erezlerin zorunlu oldu�u durum
    options.Cookie.HttpOnly = true;
});

// Authentication yap�land�rmas�
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/giris/SignIn";
        options.LogoutPath = "/giris/SignOut";
    });

// Localization (Dil deste�i) yap�land�rmas�
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "tr-TR", "en-US" };
    options.SetDefaultCulture("tr-TR")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// MVC veya Razor Pages kullan�yorsan�z ilgili servisleri ekleyin
builder.Services.AddControllersWithViews();  // E�er sadece View kullanacaksan�z

var app = builder.Build();

// Dil ayarlar�n� kullanmaya ba�lamak
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// Error handling for non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}





app.UseHttpsRedirection();
app.UseSession();
app.UseStaticFiles();

// Authentication ve Authorization s�ras�yla kullan�lmal�
app.UseAuthentication();  // Kullan�c� do�rulamas�
app.UseAuthorization();  // Yetkilendirme i�lemi

// MVC veya Razor Pages i�lemleri
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
