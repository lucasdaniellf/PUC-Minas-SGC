using AplicacaoGerenciamentoLoja.CustomParameters.Cliente;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Clientes.Application.Commands;
using Clientes.Application.Query;
using Clientes.Application.Query.DTO;
using Clientes.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AplicacaoGerenciamentoLoja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteCommandHandler _handler;
        private readonly ClienteQueryService _service;
        private readonly IAuthorizationService _authorizationService;


        public ClienteController(ClienteCommandHandler handler, ClienteQueryService service, IAuthorizationService authorizationService)
        {
            _handler = handler;
            _service = service;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.RequisitoApenasAcessoInterno)]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientes(string? nome, CancellationToken token)
        {
            IEnumerable<ClienteQueryDto> clientes = Enumerable.Empty<ClienteQueryDto>();
            if (string.IsNullOrWhiteSpace(nome))
            {
                clientes = await _service.BuscarClientes(token);
            }else
            {
                clientes = await _service.BuscarClientePorNome(nome, token);
            }

            return Ok(clientes);
        }

        [HttpGet("/Busca")]
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

            var resultado = await _authorizationService.AuthorizeAsync(User, clientes, Policies.RequisitoLerDadosCliente);
            if(!resultado.Succeeded)
            {
                return Forbid();
            }

            return Ok(clientes);
        }

        [HttpGet("{Id}", Name = "BuscarClientePorId")]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> BuscarClientePorId(string Id, CancellationToken token)
        {

            IEnumerable<ClienteQueryDto> clientes = await _service.BuscarClientePorId(Id, token);
            var resultado = await _authorizationService.AuthorizeAsync(User, clientes, Policies.RequisitoLerDadosCliente);
            if (!resultado.Succeeded)
            {
                return Forbid();
            }

            if (clientes.Any())
            {
                return Ok(clientes);
            }

            return NotFound();
        }

        //Check this
        [HttpPost]
        [Authorize(Policy = Policies.RequisitoCadastroCliente)]
        public async Task<ActionResult<IEnumerable<ClienteQueryDto>>> CadastrarCliente(CadastrarClienteCommand command, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = User.FindFirst(ClaimTypes.Email)?.Value;
                    
                    if(email != null)
                    {
                        command.AdicionarEmail(email);
                    }

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
                    IEnumerable<ClienteQueryDto> clientes = await _service.BuscarClientePorId(Id, token);
                    var resultado = await _authorizationService.AuthorizeAsync(User, clientes, Policies.RequisitoAtualizarCliente);
                    if (!resultado.Succeeded)
                    {
                        return Forbid();
                    }

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

        [HttpPatch("/Status/{Id}")]
        [Authorize(Roles = Roles.Gerente)]
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
