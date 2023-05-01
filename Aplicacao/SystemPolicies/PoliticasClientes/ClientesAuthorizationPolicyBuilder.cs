using Microsoft.AspNetCore.Authorization;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes
{

    public static class ClientesAuthorizationPolicyBuilder
    {
        public static AuthorizationPolicyBuilder AddCriarCadastroClienteAuthorizationRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new CriarCadastroClienteAuthorizationRequirement());
            return builder;
        }
        public static AuthorizationPolicyBuilder AddLerCadastroClienteAuthorizationRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new LerCadastroClienteAuthorizationRequirement());
            return builder;
        }
        public static AuthorizationPolicyBuilder AddAtualizarCadastroClienteAuthorizationRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new AtualizarCadastroClienteAuthorizationRequirement());
            return builder;
        }
    }
}
