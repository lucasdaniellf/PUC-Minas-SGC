using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasClientes
{
    public class CriarCadastroClienteAuthorizationRequirement : IAuthorizationRequirement
    {
    }
    //Valida email passado pelo usuário com email da aplicação de autenticação.
    //Gerentes tem permissão para pular esse tipo de aprovação para criações e alteração caso necessário.
    public class CriarCadastroClienteAuthorizationRequirementHandler : AuthorizationHandler<CriarCadastroClienteAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CriarCadastroClienteAuthorizationRequirement requirement, string emailUsuario)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            var isCliente = context.User.IsInRole(Roles.Cliente);
            Console.WriteLine("Inside authorization requiremente criar cadastro");

            if (context.User.IsInRole(Roles.Gerente))
            {
                context.Succeed(requirement);
            } 
            else if (isCliente)
            {
                if(string.Equals(emailUsuario, email, StringComparison.OrdinalIgnoreCase))
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
