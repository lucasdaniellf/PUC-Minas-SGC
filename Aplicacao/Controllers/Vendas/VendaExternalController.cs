using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vendas.Application.Commands;
using Vendas.Application.Query;
using Vendas.Domain;

namespace AplicacaoGerenciamentoLoja.Controllers.Vendas
{
    public partial class VendaController : ControllerBase
    {

        [HttpGet("/api/ext/vendas")]
        [Authorize(Policy = Policies.PoliticaAcessoExterno)]

        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendaCliente(CancellationToken token)
        {
            var email = BuscarEmailEmToken();
            var cliente = await _service.BuscarClientesPorEmail(email, token);

            if (!cliente.Any())
            {
                return BadRequest($"Cliente de email {email} ainda não cadastrado");
            }

            var vendas = await _service.BuscarVendasPorCliente(cliente.First().id, token);
            return Ok(vendas);
        }

        [HttpPost("/api/ext/vendas")]
        [Authorize(Policy = Policies.PoliticaAcessoExterno)]
        public async Task<ActionResult<IEnumerable<VendaDto>>> CriarVendaCliente(CancellationToken token)
        {
            try
            {
                var email = BuscarEmailEmToken();
                var cliente = await _service.BuscarClientesPorEmail(email, token);

                if (!cliente.Any())
                {
                    return BadRequest($"Cliente de email {email} ainda não cadastrado");
                }

                var command = new CriarVendaCommand()
                {
                    ClienteId = cliente.First().id
                };

                command.DefinirSolicitanteDaRequisicao(email);

                var sucesso = await _handler.Handle(command, token);
                if (sucesso)
                {
                    var vendas = await _service.BuscarVendasPorId(command.Id, token);
                    return CreatedAtAction("BuscarVendasPorId", new { Id = vendas.First().id }, vendas.First());
                }
                return NotFound();
            }
            catch (VendaException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string BuscarEmailEmToken()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return email;
        }
    }
}
