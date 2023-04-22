using Microsoft.AspNetCore.Authorization;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.ClientPolicies
{

    public static class PoliticasClientesAuthorizationPolicyBuilder
    {
        public static AuthorizationPolicyBuilder AddClienteInsertPolicyRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new LerDadosClientePolicyRequirement());
            return builder;
        }
        public static AuthorizationPolicyBuilder AddClienteReadPolicyRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new LerDadosClientePolicyRequirement());
            return builder;
        }
        public static AuthorizationPolicyBuilder AddClienteUpdatePolicyRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new LerDadosClientePolicyRequirement());
            return builder;
        }
    }
}
