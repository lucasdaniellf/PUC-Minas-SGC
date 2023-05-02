using AplicacaoGerenciamentoLoja.CustomParameters.Venda;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendas.Application.Commands;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Query;
using Vendas.Domain;
using Vendas.Domain.Model;

namespace AplicacaoGerenciamentoLoja.Vendas.Controllers
{
    [Route("api/vendas")]
    [ApiController]
    [Authorize]
    public partial class VendaController : ControllerBase
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

        //----------------------------Get---------------------------------------------//
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

        [HttpGet("formaspagamento/")]
        [AllowAnonymous]
        public ActionResult<IDictionary<int, string>> ListarFormasDePagamento()
        {
            return Ok(_service.ListarFormasDePagamento());
        }

        [HttpGet("statusvenda/")]
        [AllowAnonymous]
        public ActionResult<IDictionary<int, string>> ListarStatus()
        {
            return Ok(_service.ListarStatusDeVenda());
        }

        //---------------------------------------Updates-----------------------------------------------------------------------------------------//

        [HttpPatch("{Id}/formapagamento")]
        public async Task<ActionResult> AtualizarFormaPagamentoVenda(string Id, [FromQuery] FormaPagamentoParametro formaPagamento, CancellationToken token)
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

                    if(formaPagamento.Modalidade is not null)
                    {
                        var command = new AtualizarFormaPagamentoVendaCommand()
                        {
                            FormaDePagamento = formaPagamento.Modalidade.Value
                        };
                        command.AdicionarId(Id);

                        var sucesso = await _handler.Handle(command, token);
                        if (sucesso)
                        {
                            return NoContent();
                        }
                        return NotFound();
                    }
                    else
                    {
                        return BadRequest("Parâmetro de forma de pagamento com um valor inválido");
                    }
                }
                catch (VendaException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPatch("{Id}/statusvenda")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> ProcessarStatusVenda(string Id, [FromQuery] StatusParametro StatusVenda, CancellationToken token)
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

                switch (StatusVenda.Status)
                {
                    case StatusVendaEnum.Confirmacao:
                    {
                        sucesso = await _handler.Handle(new ConfirmarVendaCommand(Id), token);
                        break;
                    }
                    case StatusVendaEnum.Cancelamento:
                    {
                        sucesso = await _handler.Handle(new CancelarVendaCommand(Id), token);
                        break;
                    }
                    default:
                    {
                        return BadRequest("Parametro de status inválido");
                    }
                }
                
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
