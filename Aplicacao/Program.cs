using AplicacaoGerenciamentoLoja.Extensions;
using AplicacaoGerenciamentoLoja.Middlewares;
using Core.MessageBroker;
using Microsoft.OpenApi.Models;

using StackExchange.Redis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
//builder.Host.UseSerilog();

builder.Host.UseApplicationLogging();

builder.Services.AddControllers().AddJsonOptions(options =>
           options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration.GetSection("Redis")["RedisConnectionDev"]));
}
else
{
    builder.Services.AddScoped<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration.GetSection("Redis")["RedisConnection"]));
}

builder.Services.AddScoped<IMessageBrokerPublisher, RedisPublisher>();
builder.Services.AddScoped<IMessageBrokerSubscriber, RedisSubscriber>();

builder.Services.AddClienteServicesExtension(builder.Configuration);
builder.Services.AddVendasServicesExtension(builder.Configuration);
builder.Services.AddProdutosServicesExtension(builder.Configuration);
builder.Services.AddHostedServicesExtension();
builder.Services.AddAuthenticationAuthorization(builder.Configuration);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.OAuth2,
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Flows = new OpenApiOAuthFlows()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri("http://auth:8080/realms/MySGCApp/protocol/openid-connect/auth"),
                TokenUrl = new Uri("http://auth:8080/realms/MySGCApp/protocol/openid-connect/token"),
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "" }
        }
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//Custom Middlewares//
app.useExceptionHandler();
//==================//

app.UseHttpsRedirection();
app.UseAuthentication();


app.UseLoggingRequest();
//app.UseClaimsMiddlewareHandler();

app.UseAuthorization();
app.MapControllers();

app.Run();
