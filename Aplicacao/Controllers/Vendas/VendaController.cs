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
        public async Task<ActionResult> AtualizarFormaPagamentoVenda(string Id, AtualizarFormaPagamentoVendaCommand command, CancellationToken token)
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

                    command.AdicionarId(Id);
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

        [HttpPatch("{Id}/status/confirmar")]
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

        [HttpPatch("{Id}/status/cancelar")]
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
