using Clientes.Application.Query;
using Clientes.Application.Query.DTO;
using Clientes.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes
{

    public class LerDadosClientePolicyRequirement : IAuthorizationRequirement
    {
    }
    //A leitura de dados de clientes pode ser feita por qualquer usuario interno da empresa.
    //Um cliente só pode ler dados dele próprio e não deve conseguir acessar dados dos demais usuários.
    public class LerDadosClientePolicyRequirementHandler : AuthorizationHandler<LerDadosClientePolicyRequirement, IEnumerable<ClienteQueryDto>>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LerDadosClientePolicyRequirement requirement, IEnumerable<ClienteQueryDto> dtos)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var isUser = context.User.IsInRole("Cliente");

            if (isUser && !string.IsNullOrEmpty(email))
            {
                if (dtos.Any())
                {
                    if (!dtos.First().Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Succeed(requirement);
                    }
                }
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
