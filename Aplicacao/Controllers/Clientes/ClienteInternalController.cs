using AplicacaoGerenciamentoLoja.SystemPolicies;
using Clientes.Application.Commands;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clientes.Domain.Model.Status;

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
        [HttpGet("/api/int/clientes/email")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorEmail(string email, CancellationToken token)
        {
            var clientes = await _service.BuscarClientePorEmail(email, token);
            return Ok(clientes);
        }

        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
        [HttpGet("/api/int/clientes/cpf")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorCPF(string cpf, CancellationToken token)
        {

            var clientes = await _service.BuscarClientePorCPF(cpf, token);
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
        public async Task<ActionResult> AtualizarDadosCliente(string Id, AtualizarClienteCommand command, CancellationToken token)
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

        [HttpPatch("/api/int/clientes/{Id}/ativar")]
        [Authorize(Roles = Roles.Administracao)]
        public async Task<ActionResult> AtivarContaCliente(string Id, CancellationToken token)
        {
            try
            {
                AtualizarStatusClienteCommand command = new ()
                {
                    Id = Id,
                    EstaAtivo = ClienteStatus.ATIVO
                };

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
                AtualizarStatusClienteCommand command = new()
                {
                    Id = Id,
                    EstaAtivo = ClienteStatus.INATIVO
                };

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
