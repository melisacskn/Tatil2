namespace Tatil2
{
    // JWT token'ını çerezden alıp Authorization başlığına eklemek için kullanılan middleware sınıfı.
    public class JwtCookieToHeaderMiddleware
    {
        // Bir sonraki middleware işlemine yönlendirecek olan değişken
        private readonly RequestDelegate _next;

        // Middleware sınıfı oluşturulurken, bir sonraki middleware'i alıyoruz
        public JwtCookieToHeaderMiddleware(RequestDelegate next)
        {
            _next = next;  // Middleware zincirinin bir sonraki adımını kaydediyoruz
        }

        // Middleware'in çalıştığı ana metot. HTTP istekleri geldiğinde burası devreye girer
        public async Task Invoke(HttpContext context)
        {
            // Eğer header'da Authorization başlığı yoksa ve cookie'de access_token varsa
            if (!context.Request.Headers.ContainsKey("Authorization") &&
                context.Request.Cookies.TryGetValue("access_token", out var token) &&
                !string.IsNullOrEmpty(token))
            {
                // Cookie'deki access_token değerini Authorization başlığına ekliyoruz
                // Authorization başlığına "Bearer {token}" şeklinde eklenir.
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }

            // Burada _next, bir sonraki middleware veya işlemciyi çalıştırır
            // Middleware zincirinin devamını sağlarız
            await _next(context);
        }
    }
}
