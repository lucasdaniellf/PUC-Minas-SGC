using Microsoft.AspNetCore.Authorization;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public static class VendasAuthorizationPolicyBuilder
    {
        public static AuthorizationPolicyBuilder AddLerVendaAuthorizationRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new LerVendaAuthorizationRequirement());
            return builder;
        }

        public static AuthorizationPolicyBuilder AddAtualizarVendaAuthorizationRequirement(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new AtualizarVendaAuthorizationRequirement());
            return builder;
        }
    }
}
