using AplicacaoGerenciamentoLoja.Controllers.Vendas.CustomParameters;
using AplicacaoGerenciamentoLoja.Controllers.Vendas.Parametros;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vendas.Application.Commands;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Query;
using Vendas.Domain;
using static Vendas.Domain.Model.FormaPagamentoEnum;

namespace AplicacaoGerenciamentoLoja.Controllers.Vendas
{
    [Authorize(Policy = Policies.PoliticaAcessoInterno)]
    [Route("api/int/vendas")]
    [ApiController]
    public class VendaInternalController : ControllerBase
    {

        private readonly VendaQueryService _service;
        private readonly VendaCommandHandler _handler;
        private readonly IAuthorizationService _authorizationService;

        public VendaInternalController(VendaQueryService service, VendaCommandHandler handler, IAuthorizationService authorizationService)
        {
            _service = service;
            _handler = handler;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendas(string? clienteId, CancellationToken token)
        {
            IEnumerable<VendaDto> vendas;
            if (!string.IsNullOrEmpty(clienteId))
            {
                vendas = await _service.BuscarVendasPorCliente(clienteId, token);
            }
            else
            {
                vendas = await _service.BuscarVendas(token);
            }
            return Ok(vendas);
        }

        [HttpGet("{id}", Name = "BuscarVendasPorId")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendasPorId(string id, CancellationToken token)
        {
            var vendas = await _service.BuscarVendasPorId(id, token);
            var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaLerVenda);

            if (!resultado.Succeeded)
            {
                return Forbid();
            }
            return Ok(vendas);
        }

        //------------------------Custom Selects----------------------------//
        [HttpGet("periodo")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendasPorPeriodo([FromQuery] DataParametro periodo, CancellationToken token)
        {
            var vendas = await _service.BuscarVendasPorPeriodo(periodo.FormatarDataInicio(), periodo.FormatarDataFim(), token);
            return Ok(vendas);
        }

        [HttpGet("clientes")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> BuscarClientes(CancellationToken token)
        {
            var clientes = await _service.BuscarClientes(token);
            return Ok(clientes);
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> BuscarProdutos(CancellationToken token)
        {
            var produtos = await _service.BuscarProdutos(token);
            return Ok(produtos);
        }

        //------------------------Post/Updates----------------------------//
        [HttpPost]
        [Authorize(Roles = Roles.GerenteVendas)]
        public async Task<ActionResult<IEnumerable<VendaDto>>> CriarVenda(CriarVendaIntRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new CriarVendaCommand(request.ClienteId, User.FindFirstValue(ClaimTypes.Email));
                    var sucesso = await _handler.Handle(command, token);
                    if (sucesso)
                    {
                        var vendas = await _service.BuscarVendasPorId(command.Id, token);
                        return CreatedAtAction("BuscarVendasPorId", new { Id = vendas.First().Id }, vendas.First());
                    }
                    return NotFound();
                }
                catch (VendaException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPatch("/{Id}/aplicardesconto")]
        public async Task<ActionResult> ApicarDescontoVenda(string Id, int desconto, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vendas = await _service.BuscarVendasPorId(Id, token);
                    var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);

                    if (!resultado.Succeeded)
                    {
                        return Forbid();
                    }
                    var command = new AplicarDescontoVendaCommand(Id, desconto);
                    var sucesso = await _handler.Handle(command, token);

                    if (sucesso)
                    {
                        return NoContent();
                    }
                    return NotFound();
                }
                catch (VendaException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPatch("{Id}/alterarformapagamento")]
        public async Task<ActionResult> AlterarFormaPagamentoVenda(string Id, FormaPagamento formaPagamento, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vendas = await _service.BuscarVendasPorId(Id, token);
                    var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);

                    if (!resultado.Succeeded)
                    {
                        return Forbid();
                    }

                    var command = new AtualizarFormaPagamentoVendaCommand(Id, formaPagamento);
                    var sucesso = await _handler.Handle(command, token);
                    if (sucesso)
                    {
                        return NoContent();
                    }
                    return NotFound();
                }
                catch (VendaException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPatch("{Id}/confirmar")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> ConfirmarVenda(string Id, CancellationToken token)
        {
            try
            {

                var vendas = await _service.BuscarVendasPorId(Id, token);
                var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);
                var sucesso = false;

                if (!resultado.Succeeded)
                {
                    return Forbid();
                }
                sucesso = await _handler.Handle(new ConfirmarVendaCommand(Id), token);

                if (sucesso)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (VendaException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{Id}/cancelar")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> CancelarVenda(string Id, CancellationToken token)
        {
            try
            {

                var vendas = await _service.BuscarVendasPorId(Id, token);
                var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);
                var sucesso = false;

                if (!resultado.Succeeded)
                {
                    return Forbid();
                }
                sucesso = await _handler.Handle(new CancelarVendaCommand(Id), token);

                if (sucesso)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (VendaException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
