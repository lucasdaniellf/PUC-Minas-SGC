using Clientes.Application.Query.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes
{
    public class LerCadastroClienteAuthorizationRequirement : IAuthorizationRequirement
    {
    }
    //Clientes e Gerentes podem atualizar dados de clientes no sistema.
    //Um cliente não pode atualizar dados de outros clientes, somente o de si próprio
    public class LerCadastroClienteAuthorizationRequirementHandler : AuthorizationHandler<LerCadastroClienteAuthorizationRequirement, IEnumerable<ClienteQueryDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LerCadastroClienteAuthorizationRequirement requirement, IEnumerable<ClienteQueryDto> resource)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            var isCliente = context.User.IsInRole(Roles.Cliente);
            
            if (context.User.IsInRole(Roles.Gerente))
            {
                context.Succeed(requirement);
            }
            else if (isCliente)
            {
                if (resource.Any() && resource.All(c => string.Equals(c.Email, email, StringComparison.InvariantCultureIgnoreCase)))
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
