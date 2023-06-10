using AplicacaoGerenciamentoLoja.Controllers.Vendas.Parametros;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendas.Application.Commands;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Query;
using Vendas.Domain;

namespace AplicacaoGerenciamentoLoja.Controllers.Vendas
{
    [Route("api/vendas")]
    [ApiController]
    [Authorize]
    public class VendaController : ControllerBase
    {

        private readonly VendaQueryService _service;
        private readonly VendaCommandHandler _handler;
        private readonly IAuthorizationService _authorizationService;

        public VendaController(VendaQueryService service, VendaCommandHandler handler, IAuthorizationService authorizationService)
        {
            _service = service;
            _handler = handler;
            _authorizationService = authorizationService;
        }

        [HttpGet("formaspagamento/")]
        public ActionResult<IDictionary<int, string>> ListarFormasDePagamento()
        {
            return Ok(_service.ListarFormasDePagamento());
        }

        [HttpGet("statusvenda/")]
        public ActionResult<IDictionary<int, string>> ListarStatus()
        {
            return Ok(_service.ListarStatusDeVenda());
        }

        //------------------------ Item Venda --------------------------------//
        [HttpPost("{VendaId}/item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> AdicionarItemEmVenda(string VendaId, AdicionarItemEmVendaRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vendas = await _service.BuscarVendasPorId(VendaId, token);
                    var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);

                    if (!resultado.Succeeded)
                    {
                        return Forbid();
                    }

                    var command = new AdicionarItemVendaCommand(VendaId, request.ProdutoId, request.Quantidade);

                    var sucesso = await _handler.Handle(command, token);
                    if (sucesso)
                    {
                        vendas = await _service.BuscarVendasPorId(command.VendaId, token);
                        return Ok(vendas);
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

        [HttpPut("{VendaId}/item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> AtualizarItemEmVenda(string VendaId, AtualizarItemEmVendaRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vendas = await _service.BuscarVendasPorId(VendaId, token);
                    var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);

                    if (!resultado.Succeeded)
                    {
                        return Forbid();
                    }
                    var command = new AtualizarItemVendaCommand(VendaId, request.ProdutoId, request.Quantidade);

                    var sucesso = await _handler.Handle(command, token);
                    
                    if (sucesso)
                    {
                        vendas = await _service.BuscarVendasPorId(command.VendaId, token);
                        return Ok(vendas);
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

        [HttpDelete("{VendaId}/item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> RemoverItemDeVenda(string VendaId, string ProdutoId, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vendas = await _service.BuscarVendasPorId(VendaId, token);
                    var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaAtualizarVenda);

                    if (!resultado.Succeeded)
                    {
                        return Forbid();
                    }
                    var command = new RemoverItemVendaCommand(VendaId, ProdutoId);

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
