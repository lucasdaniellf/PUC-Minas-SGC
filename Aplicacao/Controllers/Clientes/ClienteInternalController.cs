using AplicacaoGerenciamentoLoja.Controllers.Clientes.Parametros.InternalController;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Clientes.Application.Commands;
using Clientes.Application.Query;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clientes.Domain.Model.ClienteStatus;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes
{
    [Authorize(Policy = Policies.PoliticaAcessoInterno)]
    public class ClienteInternalController : ControllerBase
    {
        private readonly ClienteCommandHandler _handler;
        private readonly ClienteQueryService _service;
        private readonly ILogger<ClienteInternalController> _logger;

        public ClienteInternalController(ClienteCommandHandler handler, ClienteQueryService service, ILogger<ClienteInternalController> logger)
        {
            _handler = handler;
            _service = service;
            _logger = logger;
        }


        [HttpGet("/api/int/clientes")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientes(string? nome, CancellationToken token)
        {
            IEnumerable<ClienteQueryDto> clientes;

            if (string.IsNullOrWhiteSpace(nome))
            {
                clientes = await _service.BuscarClientes(token);
            }
            else
            {
                clientes = await _service.BuscarClientePorNome(nome, token);
            }

            return Ok(clientes);
        }

        [HttpGet("/api/int/clientes/email")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorEmail(string email, CancellationToken token)
        {
            var clientes = await _service.BuscarClientePorEmail(email, token);
            return Ok(clientes);
        }

        [HttpGet("/api/int/clientes/cpf")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorCPF(string cpf, CancellationToken token)
        {

            var clientes = await _service.BuscarClientePorCPF(cpf, token);
            return Ok(clientes);
        }

        [HttpGet("/api/int/clientes/{Id}", Name = "BuscarClientePorId")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorId(string Id, CancellationToken token)
        {

            IEnumerable<ClienteQueryDto> clientes = await _service.BuscarClientePorId(Id, token);
            if (clientes.Any())
            {
                return Ok(clientes);
            }

            return NotFound();
        }

        //=====================================================================================================================================================================//
        [HttpPost("/api/int/clientes")]
        [Authorize(Roles = Roles.GerenteVendas)]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> CadastrarCliente([FromBody] CadastrarClienteInternalRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CadastrarClienteCommand command = new(request.Nome, request.Cpf, request.Endereco, request.Email);

                    var success = await _handler.Handle(command, token);
                    if (success)
                    {
                        return CreatedAtAction(nameof(BuscarClientePorId), new { Id = command.Id.ToString() }, await _service.BuscarClientePorId(command.Id, token));
                    }
                }
                catch (ClienteException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPut("/api/int/clientes/{Id}")]
        [Authorize(Roles = Roles.GerenteVendas)]
        public async Task<ActionResult> AtualizarDadosCliente(string Id, [FromBody] AtualizarClienteInternalRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AtualizarClienteCommand command = new(Id, request.Nome, request.Cpf, request.Endereco, request.Email);


                    bool success = await _handler.Handle(command, token);
                    if (!success)
                    {
                        return NotFound();
                    }
                    return NoContent();
                }
                catch (ClienteException ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return BadRequest();
        }

        [HttpPatch("/api/int/clientes/{Id}/ativar")]
        [Authorize(Roles = Roles.Administracao)]
        public async Task<ActionResult> AtivarContaCliente(string Id, CancellationToken token)
        {
            try
            {
                AtualizarStatusClienteCommand command = new (Id, ClienteStatusEnum.ATIVO);

                bool success = await _handler.Handle(command, token);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ClienteException ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPatch("/api/int/clientes/{Id}/desativar")]
        [Authorize(Roles = Roles.Administracao)]
        public async Task<ActionResult> DesativarContaCliente(string Id, CancellationToken token)
        {
            try
            {
                AtualizarStatusClienteCommand command = new(Id, ClienteStatusEnum.INATIVO);

                bool success = await _handler.Handle(command, token);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ClienteException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
