using AplicacaoGerenciamentoLoja.SystemPolicies;
using AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.Extensions
{
    public static class AuthenticationAuthorizationExtensionServices
    {
        public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.Authority = configuration.GetSection("Authentication")["Authority"];
                opt.Audience = configuration.GetSection("Authentication")["Audience"];
                opt.RequireHttpsMetadata = false;
            });

            services.AddSingleton<IAuthorizationRequirement, AtualizarVendaAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, AtualizarVendaAuthorizationRequirementHandler>();

            services.AddSingleton<IAuthorizationRequirement, LerVendaAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, LerVendaAuthorizationRequirementHandler>();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(Policies.PoliticaAtualizarVenda, policy => policy.AddAtualizarVendaAuthorizationRequirement());
                opt.AddPolicy(Policies.PoliticaLerVenda, policy => policy.AddLerVendaAuthorizationRequirement());

                opt.AddPolicy(Policies.PoliticaGerenciamentoProduto, policy => policy.RequireRole(Roles.GerenteProdutos));
                
                opt.AddPolicy(Policies.PoliticaAcessoInterno, policy => policy.RequireAssertion(context => context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value != Roles.Cliente)));
                opt.AddPolicy(Policies.PoliticaAcessoExterno, policy => policy.RequireAssertion(context => !context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value != Roles.Cliente) && !string.IsNullOrWhiteSpace(context.User.FindFirstValue(ClaimTypes.Email))));
            });

            return services;
        }
    }
}
