using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vendas.Application.Query;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.SalePolicies
{
    public class LerVendaPolicyRequirement : IAuthorizationRequirement
    {
    }

    //Verifica se o cliente é dono da venda retornada na consulta. Todos os outros tipos de usuário podem acessar o recurso.
    public class LerVendaPolicyRequirementHandler : AuthorizationHandler<LerVendaPolicyRequirement, IEnumerable<VendaDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LerVendaPolicyRequirement requirement, IEnumerable<VendaDto> resource)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var isCliente = context.User.IsInRole("Cliente");

            if (!string.IsNullOrWhiteSpace(email))
            {
                if (isCliente)
                {
                    //Possivel ponto de melhoria de performance, seria importante verificar outra lógica em vez de utilizar o All//
                    if (resource.All(venda => string.Equals(venda.cliente.Email, email, StringComparison.InvariantCultureIgnoreCase)))
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
                    context.Succeed(requirement);

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
