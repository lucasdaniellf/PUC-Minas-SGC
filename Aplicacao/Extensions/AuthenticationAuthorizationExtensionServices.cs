using AplicacaoGerenciamentoLoja.SystemPolicies;
using AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes;
using AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            services.AddAuthorization(opt =>
            {
                //opt.AddPolicy("admin", x => { x.RequireClaim("usertype", "admin"); }) ;
                opt.AddPolicy(Policies.RequisitoLerDadosCliente, policy => policy.AddClienteReadPolicyRequirement());
                opt.AddPolicy(Policies.RequisitoAtualizarCliente, policy => policy.AddClienteUpdatePolicyRequirement());
                opt.AddPolicy(Policies.RequisitoCadastroCliente, policy => policy.AddClienteInsertPolicyRequirement());

                opt.AddPolicy(Policies.RequisitoApenasAcessoInterno, policy => policy.RequireAssertion(context => context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Any(r => r.Value != Roles.Cliente)));

                opt.AddPolicy(Policies.RequisitoLerDadosVenda, policy => policy.AddLerVendaPolicyRequirement());
                opt.AddPolicy(Policies.RequisitoCadastrarVenda, policy => policy.AddCriarVendaPolicyRequirement());
                opt.AddPolicy(Policies.RequisitoAtualizarVenda, policy => policy.AddAtualizarVendaPolicyRequirement());

                opt.AddPolicy(Policies.RequisitoGerenciamentoProduto, policy => policy.RequireRole(Roles.Gerente));
            });

            return services;
        }
    }
}
