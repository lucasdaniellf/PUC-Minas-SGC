using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace AplicacaoGerenciamentoLoja.Middlewares
{
    public class ClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;

            var userClaims = new List<string>();
            if (identity != null)
            {
                var claims = identity.Claims;
                foreach (var claim in claims)
                {
                    userClaims.Add(claim.ToString());
                }
            }
            await _next(context);
            var result = JsonSerializer.Serialize(userClaims, new JsonSerializerOptions() { WriteIndented = true });
            //WriteAsJsonAsync changes headers (Content-Type) and throws exceptions
            Console.WriteLine("Claims:\n" + result);
        }
    }


    public static class ClaimsMiddlewareExtension
    {
        public static IApplicationBuilder UseClaimsMiddlewareHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClaimsMiddleware>();
        }
    }
}
