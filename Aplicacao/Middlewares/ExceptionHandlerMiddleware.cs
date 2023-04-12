using Newtonsoft.Json;
using System.Net;

namespace AplicacaoGerenciamentoLoja.Middlewares
{
    public class ExceptionHandlerMiddleware
    {

        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Log from Middleware");
                Console.WriteLine(string.Concat(ex.Message, " - ", ex.StackTrace));

                var result = JsonConvert.SerializeObject(new { erro = "Houve um erro inesperado. Comunique o suporte." });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync(result);
            }
        }
    }


    public static class ExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder useExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
