using AplicacaoGerenciamentoLoja.Controllers.Clientes.Parametros.ExternalController;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Clientes.Application.Commands;
using Clientes.Application.Query;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes
{
    [Authorize(Policy = Policies.PoliticaAcessoExterno)]
    public class ClientesExternalController : ControllerBase
    {
        private readonly ClienteCommandHandler _handler;
        private readonly ClienteQueryService _service;
        private readonly ILogger<ClientesExternalController> _logger;

        public ClientesExternalController(ClienteCommandHandler handler, ClienteQueryService service, ILogger<ClientesExternalController> logger)
        {
            _handler = handler;
            _service = service;
            _logger = logger;
        }


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
        
        [HttpPost("/api/ext/cliente")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> CadastrarCliente([FromBody] CadastrarClienteExternalRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    CadastrarClienteCommand command = new(request.Nome, request.Cpf, request.Endereco, BuscarEmailEmToken());

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

        [HttpPut("/api/ext/cliente")]
        public async Task<ActionResult> AtualizarCliente([FromBody] AtualizarClienteExternalRequest request, CancellationToken token)
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

                    AtualizarClienteCommand command = new(clientes.First().Id, request.Nome, request.Cpf, request.Endereco, BuscarEmailEmToken());

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
