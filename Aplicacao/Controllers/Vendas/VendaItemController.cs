using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Mvc;
using Vendas.Application.Commands;
using Vendas.Application.Query;
using Vendas.Domain;

namespace AplicacaoGerenciamentoLoja.Controllers.Vendas
{
    public partial class VendaController : ControllerBase
    {
        //------------------------ Item Venda --------------------------------//
        [HttpPost("{VendaId}/item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> AdicionarItemEmVenda(string VendaId, AdicionarItemVendaCommand item, CancellationToken token)
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

                    item.AdicionarVendaId(VendaId);

                    var sucesso = await _handler.Handle(item, token);
                    if (sucesso)
                    {
                        vendas = await _service.BuscarVendasPorId(item.VendaId, token);
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

        [HttpPut("{VendaId}/item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> AtualizarItemEmVenda(string VendaId, AtualizarItemVendaCommand item, CancellationToken token)
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

                    item.AdicionarVendaId(VendaId);

                    var sucesso = await _handler.Handle(item, token);
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

        [HttpDelete("{VendaId}/item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> RemoverItemDeVenda(string VendaId, RemoverItemVendaCommand item, CancellationToken token)
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

                    item.AdicionarVendaId(VendaId);

                    var sucesso = await _handler.Handle(item, token);
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
