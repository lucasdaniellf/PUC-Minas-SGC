using AplicacaoGerenciamentoLoja.SystemPolicies;
using Clientes.Application.Commands;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes
{
    public partial class ClientesController : ControllerBase
    {
        [Authorize(Policy = Policies.PoliticaAcessoExterno)]
        [HttpGet("/api/ext/cliente", Name = "BuscarCliente")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarCliente(CancellationToken token)
        {
            var email = BuscarEmailEmToken();
            var clientes = await _service.BuscarClientePorEmail(email, token);

            if (clientes.Any())
            {
                return Ok(clientes);
            }
            else
            {
                return BadRequest($"Cliente de email {email} ainda não está cadastrado");
            }
        }
        
        [Authorize(Policy = Policies.PoliticaAcessoExterno)]
        [HttpPost("/api/ext/cliente")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> CadastrarCliente(CriarClienteRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    CadastrarClienteCommand command = new()
                    {
                        Nome = request.Nome,
                        Cpf = request.Cpf,
                        Endereco = request.Endereco,
                        Email = BuscarEmailEmToken()
                    };

                    var success = await _handler.Handle(command, token);
                    if (success)
                    {
                        return CreatedAtAction(nameof(BuscarCliente), await _service.BuscarClientePorId(command.Id, token));
                    }
                }
                catch (ClienteException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [Authorize(Policy = Policies.PoliticaAcessoExterno)]
        [HttpPut("/api/ext/cliente")]
        public async Task<ActionResult> AtualizarCliente(AtualizarClienteRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = BuscarEmailEmToken();
                    IEnumerable<ClienteQueryDto> clientes = await _service.BuscarClientePorEmail(email, token);

                    if (!clientes.Any())
                    {
                        return BadRequest($"Cliente de email {email} ainda não está cadastrado");
                    }

                    AtualizarClienteCommand command = new()
                    {
                        Nome = request.Nome,
                        Cpf = request.Cpf,
                        Endereco = request.Endereco,
                        Email = BuscarEmailEmToken(),
                    };

                    command.AdicionarId(clientes.First().Id);
                    await _handler.Handle(command, token);

                    return NoContent();
                }
                catch (ClienteException ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return BadRequest();
        }

        private string BuscarEmailEmToken()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return email;
        }
    }
}
