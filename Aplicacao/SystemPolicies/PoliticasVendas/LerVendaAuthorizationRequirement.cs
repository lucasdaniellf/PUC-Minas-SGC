using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vendas.Application.Query;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public class LerVendaAuthorizationRequirement : IAuthorizationRequirement
    {
    }
    //Verifica se o usuário é cliente e dono da venda retornada na consulta ou gerente.
    public class LerVendaAuthorizationRequirementHandler : AuthorizationHandler<LerVendaAuthorizationRequirement, IEnumerable<VendaDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LerVendaAuthorizationRequirement requirement, IEnumerable<VendaDto> resource)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            
            if (context.User.IsInRole(Roles.Gerente))
            {
                context.Succeed(requirement);
            }
            else if (context.User.IsInRole(Roles.Cliente))
            {
                //Possivel ponto de melhoria de performance, seria importante verificar outra lógica em vez de utilizar o All//
                if (resource.Any() && resource.All(venda => string.Equals(venda.cliente.Email, email, StringComparison.InvariantCultureIgnoreCase)))
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
