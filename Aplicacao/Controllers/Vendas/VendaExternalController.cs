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
    [Authorize(Policy = Policies.PoliticaAcessoExterno)]
    [ApiController]
    [Route("/api/ext/vendas")]
    public class VendaExternalController : ControllerBase
    {

        private readonly VendaQueryService _service;
        private readonly VendaCommandHandler _handler;
        private readonly IAuthorizationService _authorizationService;

        public VendaExternalController(VendaQueryService service, VendaCommandHandler handler, IAuthorizationService authorizationService)
        {
            _service = service;
            _handler = handler;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendas(CancellationToken token)
        {
            var email = BuscarEmailEmToken();
            var cliente = await _service.BuscarClientesPorEmail(email, token);

            if (!cliente.Any())
            {
                return BadRequest($"Cliente de email {email} ainda não cadastrado");
            }

            var vendas = await _service.BuscarVendasPorCliente(cliente.First().Id, token);
            return Ok(vendas);
        }

        [HttpGet("{id}", Name = "BuscarVendaClientePorId")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendaClientePorId(string id, CancellationToken token)
        {
            var vendas = await _service.BuscarVendasPorId(id, token);
            var resultado = await _authorizationService.AuthorizeAsync(User, vendas, Policies.PoliticaLerVenda);

            if (!resultado.Succeeded)
            {
                return Forbid();
            }
            return Ok(vendas);
        }

        //------------------------Post/Updates----------------------------//
        [HttpPost]
        public async Task<ActionResult<IEnumerable<VendaDto>>> CriarVenda(CancellationToken token)
        {
            try
            {
                var email = BuscarEmailEmToken();
                var cliente = await _service.BuscarClientesPorEmail(email, token);

                if (!cliente.Any())
                {
                    return BadRequest($"Cliente de email {email} ainda não cadastrado");
                }

                var command = new CriarVendaCommand(cliente.First().Id, email);
                var sucesso = await _handler.Handle(command, token);
                if (sucesso)
                {
                    var vendas = await _service.BuscarVendasPorId(command.Id, token);
                    return CreatedAtAction("BuscarVendaClientePorId", new { Id = vendas.First().Id }, vendas.First());
                }
                return NotFound();
            }
            catch (VendaException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //O FormaDePagamento retorna um statusCode 200 com conteúdo porque alterar a forma de pagamento atualiza o desconto aplicado, sendo necessário informar
        //ao usuário que houve atualização no preço
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
                        vendas = await _service.BuscarVendasPorId(Id, token);
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

        private string BuscarEmailEmToken()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return email;
        }
    }
}
