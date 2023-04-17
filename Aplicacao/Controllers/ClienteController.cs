using AplicacaoGerenciamentoLoja.CustomParameters.Cliente;
using Clientes.Application.Commands;
using Clientes.Application.Query;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AplicacaoGerenciamentoLoja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteCommandHandler _handler;
        private readonly ClienteQueryService _service;

        public ClienteController(ClienteCommandHandler handler, ClienteQueryService service)
        {
            _handler = handler;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientes([FromQuery] CustomClientFilter filter, CancellationToken token)
        {
            IEnumerable<ClienteQueryDto> clientes = Enumerable.Empty<ClienteQueryDto>();
            if (filter.Filtro == null && string.IsNullOrEmpty(filter.Valor))
            {
                clientes = await _service.BuscarClientes(token);
            }
            else
            {
                if(string.IsNullOrEmpty(filter.Valor) || filter.Filtro == null)
                {
                    return BadRequest();
                }

                switch ((int)filter.Filtro)
                {
                    case 0:
                    {
                        clientes = await _service.BuscarClientePorNome(filter.Valor, token);
                        break;
                    }
                    case 1:
                    {
                        clientes = await _service.BuscarClientePorCPF(filter.Valor, token);
                        break;
                    }
                    case 2:
                    {
                        clientes = await _service.BuscarClientePorEmail(filter.Valor, token);
                        break;
                    }
                }
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

        [HttpPut("{Id}")]
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
    }
}
