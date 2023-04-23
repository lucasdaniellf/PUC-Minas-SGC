using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vendas.Application.Query;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public class AtualizarVendaPolicyRequirement : IAuthorizationRequirement
    {
    }
    //Verifica se o cliente é dono da venda retornada na consulta. Somente clientes podem atualizar suas próprias vendas.//
    public class AtualizarVendaPolicyRequirementHandler : AuthorizationHandler<AtualizarVendaPolicyRequirement, IEnumerable<VendaDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AtualizarVendaPolicyRequirement requirement, IEnumerable<VendaDto> resource)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var isCliente = context.User.IsInRole("Cliente");

            if (email != null && isCliente)
            {
                if (resource.Any(venda => venda.cliente.Email == email))
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
