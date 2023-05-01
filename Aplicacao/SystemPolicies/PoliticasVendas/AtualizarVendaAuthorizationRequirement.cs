﻿using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vendas.Application.Query;

namespace AplicacaoGerenciamentoLoja.SystemPolicies.PoliticasVendas
{
    public class AtualizarVendaAuthorizationRequirement : IAuthorizationRequirement
    {
    }
    //Verifica se o usuário é cliente e dono da venda retornada na consulta ou gerente.
    //Se não houver vendas com o id informado, retorna sucesso caso gerente, mas falha caso cliente para manter a consistência com as demais políticas do sistema.
    //Cliente deve receber um Fail() mesmo se a venda não existir, o que não é o caso para gerente (receberá um 404)
    public class AtualizarVendaAuthorizationRequirementHandler : AuthorizationHandler<AtualizarVendaAuthorizationRequirement, IEnumerable<VendaDto>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AtualizarVendaAuthorizationRequirement requirement, IEnumerable<VendaDto> resource)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            //Possivel ponto de melhoria de performance, seria importante verificar outra lógica em vez de utilizar o All//
            var validacao = resource.All(venda => string.Equals(venda.criadoPor, email, StringComparison.OrdinalIgnoreCase));

            if(context.User.IsInRole(Roles.Gerente) && validacao)  
            {
                context.Succeed(requirement);
            }
            else if (context.User.IsInRole(Roles.Cliente))
            {
                if (resource.Any() && validacao)
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
