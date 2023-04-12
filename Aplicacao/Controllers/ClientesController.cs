using Clientes.Application.Commands;
using Clientes.Application.Query;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AplicacaoGerenciamentoLoja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteCommandHandler _handler;
        private readonly ClienteQueryService _service;

        public ClientesController(ClienteCommandHandler handler, ClienteQueryService service)
        {
            _handler = handler;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientes(string? cpf, string? nome, CancellationToken token)
        {
            IEnumerable<ClienteQueryDto> clientes;
            if (!string.IsNullOrEmpty(cpf))
            {
                clientes = await _service.BuscarClientePorCPF(cpf, token);
                if (clientes.Any())
                {
                    return Ok(clientes);
                }

                return NotFound();
            }
            else if (string.IsNullOrEmpty(nome))
            {
                clientes = await _service.BuscarClientes(token);
            }
            else
            {
                clientes = await _service.BuscarClientePorNome(nome, token);
            }
            return Ok(clientes);
        }

        [HttpGet("{Id}", Name = "BuscarClientePorId")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorId(string Id, CancellationToken token)
        {
            IEnumerable<ClienteQueryDto> clientes = await _service.BuscarClientePorId(Id, token);
            if (clientes.Any())
            {
                return Ok(clientes);
            }

            return NotFound();
        }

        //Check this
        [HttpPost]
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

        [HttpPut]
        public async Task<ActionResult> AtualizarCadastroCliente(AtualizarCadastroClienteCommand command, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

        [HttpPut("conta/ativar")]
        public async Task<ActionResult> AtivarContaCliente(AtivarClienteCommand command, CancellationToken token)
        {
            try
            {
                bool success = await _handler.Handle(command, token);
                if (success)
                {
                    return NoContent();
                }
            }
            catch (ClienteException ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound();
        }

        [HttpPut("conta/desativar")]
        public async Task<ActionResult> DesativarContaCliente(InativarClienteCommand command, CancellationToken token)
        {
            try
            {
                bool success = await _handler.Handle(command, token);
                if (success)
                {
                    return NoContent();
                }
            }
            catch (ClienteException ex)
            {
                return BadRequest(ex.Message);

            }
            return NotFound();
        }
    }
}
