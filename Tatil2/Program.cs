using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tatil2.DBContext; // Konsol loglama için gerekli namespace

var builder = WebApplication.CreateBuilder(args);

// Logging yapýlandýrmasýný ekleyin
builder.Logging.ClearProviders();
builder.Logging.AddConsole();  // Konsola loglama ekleyin

// DbContext'inizi ekleyin
builder.Services.AddDbContext<TatilDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Diðer servis eklemeleri...
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Tatil2.Session"; // Çerez adý
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.IsEssential = true; // Çerezlerin zorunlu olduðu durum
    options.Cookie.HttpOnly = true;
});

// Authentication yapýlandýrmasý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/giris/SignIn";
        options.LogoutPath = "/giris/SignOut";
    });

// Localization (Dil desteði) yapýlandýrmasý
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "tr-TR", "en-US" };
    options.SetDefaultCulture("tr-TR")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// MVC veya Razor Pages kullanýyorsanýz ilgili servisleri ekleyin
builder.Services.AddControllersWithViews();  // Eðer sadece View kullanacaksanýz

var app = builder.Build();

// Dil ayarlarýný kullanmaya baþlamak
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

// Authentication ve Authorization sýrasýyla kullanýlmalý
app.UseAuthentication();  // Kullanýcý doðrulamasý
app.UseAuthorization();  // Yetkilendirme iþlemi

// MVC veya Razor Pages iþlemleri
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
