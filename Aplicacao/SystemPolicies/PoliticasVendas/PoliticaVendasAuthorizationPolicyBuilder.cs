using Microsoft.AspNetCore.Authorization;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public static class PoliticaVendasAuthorizationPolicyBuilder
    {
        public static AuthorizationPolicyBuilder AddCriarVendaPolicyRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new CriarVendaPolicyRequirement());
            return builder;
        }

        public static AuthorizationPolicyBuilder AddAtualizarVendaPolicyRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new AtualizarVendaPolicyRequirement());
            return builder;
        }

        public static AuthorizationPolicyBuilder AddLerVendaPolicyRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new LerVendaPolicyRequirement());
            return builder;
        }
    }
}
