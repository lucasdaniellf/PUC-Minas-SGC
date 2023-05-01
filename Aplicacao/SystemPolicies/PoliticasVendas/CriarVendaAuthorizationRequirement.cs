using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vendas.Application.Query;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public class CriarVendaAuthorizationRequirement : IAuthorizationRequirement
    {
    }

    //Verifica se a requisição foi solicitada por um cliente e se este cliente é o usuário da requisição. 
    //Um cliente só pode solicitar a criação de vendas para ele próprio.
    public class CriarVendaAuthorizationRequirementHandler : AuthorizationHandler<CriarVendaAuthorizationRequirement, IEnumerable<ClienteDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CriarVendaAuthorizationRequirement requirement, IEnumerable<ClienteDto> resource)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (context.User.IsInRole(Roles.Gerente))
            {
                context.Succeed(requirement);
            }
            else if (context.User.IsInRole(Roles.Cliente))
            {
                if (resource.Any() && resource.All(dto => string.Equals(email, dto.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
            
            return Task.CompletedTask;
        }
    }
}
