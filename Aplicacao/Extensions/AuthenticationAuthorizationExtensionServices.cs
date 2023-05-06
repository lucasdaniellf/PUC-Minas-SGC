using AplicacaoGerenciamentoLoja.SystemPolicies;
using AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes;
using AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

            services.AddSingleton<IAuthorizationRequirement, CriarCadastroClienteAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, CriarCadastroClienteAuthorizationRequirementHandler>();

            services.AddSingleton<IAuthorizationRequirement, AtualizarCadastroClienteAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, AtualizarCadastroClienteAuthorizationRequirementHandler>();

            services.AddSingleton<IAuthorizationRequirement, LerCadastroClienteAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, LerCadastroClienteAuthorizationRequirementHandler>();

            services.AddSingleton<IAuthorizationRequirement, CriarVendaAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, CriarVendaAuthorizationRequirementHandler>();

            services.AddSingleton<IAuthorizationRequirement, AtualizarVendaAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, AtualizarVendaAuthorizationRequirementHandler>();

            services.AddSingleton<IAuthorizationRequirement, LerVendaAuthorizationRequirement>();
            services.AddSingleton<IAuthorizationHandler, LerVendaAuthorizationRequirementHandler>();


            services.AddAuthorization(opt =>
            {
                //opt.AddPolicy("admin", x => { x.RequireClaim("usertype", "admin"); }) ;
                opt.AddPolicy(Policies.PoliticaCriarCadastroCliente, policy => policy.AddCriarCadastroClienteAuthorizationRequirement());
                opt.AddPolicy(Policies.PoliticaAtualizarCadastroCliente, policy => policy.AddAtualizarCadastroClienteAuthorizationRequirement());
                opt.AddPolicy(Policies.PoliticaLerCadastroCliente, policy => policy.AddLerCadastroClienteAuthorizationRequirement());


                opt.AddPolicy(Policies.PoliticaCriarVenda, policy => policy.AddCriarVendaAuthorizationRequirement());
                opt.AddPolicy(Policies.PoliticaAtualizarVenda, policy => policy.AddAtualizarVendaAuthorizationRequirement());
                opt.AddPolicy(Policies.PoliticaLerVenda, policy => policy.AddLerVendaAuthorizationRequirement());

                opt.AddPolicy(Policies.PoliticaGerenciamentoProduto, policy => policy.RequireRole(Roles.Gerente));
                
                opt.AddPolicy(Policies.PoliticaAcessoInterno, policy => policy.RequireAssertion(context => context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value != Roles.Cliente)));
                
                opt.AddPolicy(Policies.PoliticaValidacaoEmailUsuario, policy => policy.RequireAssertion(context => !string.IsNullOrWhiteSpace(context.User.FindFirst(ClaimTypes.Email)?.Value)));
            });

            return services;
        }
    }
}
