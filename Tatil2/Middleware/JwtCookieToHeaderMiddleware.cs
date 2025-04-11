namespace Tatil2
{
    public class JwtCookieToHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieToHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Eğer header'da Authorization zaten yoksa ve cookie'de access_token varsa
            if (!context.Request.Headers.ContainsKey("Authorization") &&
                context.Request.Cookies.TryGetValue("access_token", out var token) &&
                !string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }

            await _next(context);
        }
    }

}
