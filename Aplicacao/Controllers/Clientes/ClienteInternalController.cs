using AplicacaoGerenciamentoLoja.CustomParameters.Cliente;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Clientes.Application.Commands;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes
{
    public partial class ClientesController : ControllerBase
    {

        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
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

        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
        [HttpGet("/api/int/clientes/busca")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorFiltro([FromQuery] CustomClientFilter filter, CancellationToken token)
        {
            IEnumerable<ClienteQueryDto> clientes = Enumerable.Empty<ClienteQueryDto>();

            switch ((int)filter.Filtro)
            {
                case 0:
                    {
                        clientes = await _service.BuscarClientePorCPF(filter.Valor, token);
                        break;
                    }
                case 1:
                    {
                        clientes = await _service.BuscarClientePorEmail(filter.Valor, token);
                        break;
                    }
            }

            return Ok(clientes);
        }

        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
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
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> CadastrarCliente(CadastrarClienteCommand command, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
        public async Task<ActionResult> AtualizarCliente(string Id, AtualizarClienteCommand command, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    command.AdicionarId(Id);
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

        [HttpPatch("/api/int/clientes/{Id}/status")]
        [Authorize(Roles = Roles.Administracao)]
        public async Task<ActionResult> AlterarStatusCliente(string Id, AtualizarStatusClienteCommand command, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    command.AdicionarId(Id);
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
    }
}
