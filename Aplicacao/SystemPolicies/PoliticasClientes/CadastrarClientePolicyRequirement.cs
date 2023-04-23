using Clientes.Application.Query.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes
{
    public class CadastrarClientePolicyRequirement : IAuthorizationRequirement
    {
    }
    //Somente clientes com email válido no token de requisição devem poder se cadastrar no sistema.
    public class CadastrarClientePolicyRequirementHandler : AuthorizationHandler<CadastrarClientePolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CadastrarClientePolicyRequirement requirement)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email) || !context.User.IsInRole("Cliente"))
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
