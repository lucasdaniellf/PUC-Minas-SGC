using AplicacaoGerenciamentoLoja.CustomParameters.Venda;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vendas.Application.Commands;
using Vendas.Application.Query;
using Vendas.Domain;
using static Vendas.Domain.Model.FormaPagamentoEnum;

namespace AplicacaoGerenciamentoLoja.Vendas.Controllers
{
    public partial class VendaController : ControllerBase
    {

        [HttpGet("/api/int/vendas/")]
        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
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

        [HttpGet("/api/int/vendas/periodo")]
        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendasPorPeriodo([FromQuery] DataParametro periodo, CancellationToken token)
        {
            var vendas = await _service.BuscarVendasPorPeriodo(periodo.FormatarDataInicio(), periodo.FormatarDataFim(), token);
            return Ok(vendas);
        }

        [HttpGet("/api/int/vendas/clientes")]
        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> BuscarClientes(CancellationToken token)
        {
            var clientes = await _service.BuscarClientes(token);
            return Ok(clientes);
        }

        [HttpGet("/api/int/vendas/produtos")]
        [Authorize(Policy = Policies.PoliticaAcessoInterno)]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> BuscarProdutos(CancellationToken token)
        {
            var produtos = await _service.BuscarProdutos(token);
            return Ok(produtos);
        }


        [HttpPost("/api/int/vendas/")]
        [Authorize(Roles = Roles.GerenteVendas)]
        public async Task<ActionResult<IEnumerable<VendaDto>>> CriarVenda(CriarVendaCommand venda, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    venda.DefinirSolicitanteDaRequisicao(User.FindFirstValue(ClaimTypes.Email));
                    var sucesso = await _handler.Handle(venda, token);
                    if (sucesso)
                    {
                        var vendas = await _service.BuscarVendasPorId(venda.Id, token);
                        return CreatedAtAction("BuscarVendasPorId", new { Id = vendas.First().id }, vendas.First());
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

        [HttpPatch("/api/int/vendas/{Id}/desconto")]
        public async Task<ActionResult> ApicarDescontoVenda(string Id, int valor, CancellationToken token)
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

                    var command = new AplicarDescontoVendaCommand()
                    {
                        Id = Id,
                        Desconto = valor,
                    };

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
    }
}
