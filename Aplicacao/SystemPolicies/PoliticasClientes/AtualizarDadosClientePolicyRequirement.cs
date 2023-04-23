using Clientes.Application.Query.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes
{
    public class AtualizarDadosClientePolicyRequirement : IAuthorizationRequirement
    {
    }
    //Somente clientes podem atualizar seus dados no sistema.
    //Um cliente não deve poder atualizar dados de outros clientes
    public class AtualizarDadosClientePolicyRequirementHandler : AuthorizationHandler<AtualizarDadosClientePolicyRequirement, IEnumerable<ClienteQueryDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AtualizarDadosClientePolicyRequirement requirement, IEnumerable<ClienteQueryDto> resource)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var isUser = context.User.IsInRole("Cliente");

            if (email != null && isUser)
            {
                if (resource.Any(c => c.Email == email))
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
