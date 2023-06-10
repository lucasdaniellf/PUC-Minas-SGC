using Core.Messages.Commands;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Polly;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace AplicacaoGerenciamentoLoja.Middlewares
{
    public class LoggingRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingRequestMiddleware> _logger;
        public LoggingRequestMiddleware(RequestDelegate next, ILogger<LoggingRequestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestBody;
            context.Request.EnableBuffering();

            MemoryStream requestStream = new ();
            await context.Request.Body.CopyToAsync(requestStream);
            context.Request.Body.Position = 0;
            using (var reader = new StreamReader(requestStream))
            {
                requestStream.Position = 0;
                requestBody = await reader.ReadToEndAsync();
            }
           
            //outras opções:
            //utilizar using(var reader = new StreamReader(context.Request.Body){} finaliza reader e body stream. Se o body stream for utilizado posteriormente, haverá erro (trying to access disposed object)
            //por isso é necessário copiá-lo para um novo stream

            long start = Stopwatch.GetTimestamp();
            var email = context.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var correlationId = Guid.NewGuid().ToString();
            
            LogContext.PushProperty("CorrelationId", correlationId);
            LogContext.PushProperty("Usuario", email);

            string request = JsonConvert.SerializeObject(new
            {
                RequestMethod = context.Request.Method,
                RequestPath = context.Request.Path.ToString(),
                Url = context.Request.GetDisplayUrl(),
                RequestBody = requestBody
            });

            _logger.LogInformation("Request: {request}", request);

            await _next(context);
            double elapsedMilliseconds = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

            string response = JsonConvert.SerializeObject(new
            {
                RequestMethod = context.Request.Method,
                RequestPath = context.Request.Path.ToString(),
                context.Response.StatusCode,
                ElapsedTime = string.Concat(elapsedMilliseconds.ToString("0.000"), " ms")
            });

            var loglevel = context.Response.StatusCode > 499 ? LogLevel.Error : LogLevel.Information;
            _logger.Log(loglevel, "Response: {response}", response);
        }
        //From RequestLoggingMiddleware - Serilog
        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (double)((stop - start) * 1000) / (double)Stopwatch.Frequency;
        }
    }


    public static class LoggingRequestMiddlewareExtension
    {
        public static IApplicationBuilder UseLoggingRequest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingRequestMiddleware>();
        }
    }
}
