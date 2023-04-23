using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vendas.Application.Query;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public class CriarVendaPolicyRequirement : IAuthorizationRequirement
    {
    }

    //Verifica se a requisição foi solicitada por um cliente e se este cliente é o usuário da requisição. 
    //Um cliente só pode solicitar a criação de vendas para ele próprio.
    public class CriarVendaPolicyRequirementyHandler : AuthorizationHandler<CriarVendaPolicyRequirement, IEnumerable<ClienteDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CriarVendaPolicyRequirement requirement, IEnumerable<ClienteDto> resource)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var isUser = context.User.IsInRole("Cliente");

            if (email != null && isUser)
            {
                if (resource.Any(dto => dto.Email == email))
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
