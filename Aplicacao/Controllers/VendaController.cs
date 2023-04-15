using AplicacaoGerenciamentoLoja.CustomParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vendas.Application.Commands;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Query;
using Vendas.Domain;
using Vendas.Domain.Model;

namespace AplicacaoGerenciamentoLoja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendaController : ControllerBase
    {

        private readonly VendaQueryService _service;
        private readonly VendaCommandHandler _handler;

        public VendaController(VendaQueryService service, VendaCommandHandler handler)
        {
            _service = service;
            _handler = handler;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendas(string? clienteId, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(clienteId))
            {
                return Ok(await _service.BuscarVendasPorCliente(clienteId, token));
            }
            else
            {
                return Ok(await _service.BuscarVendas(token));
            }
        }

        [HttpGet("Periodo/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendasPorPeriodo([FromQuery] CustomDate periodo, CancellationToken token)
        {
            var vendas = await _service.BuscarVendasPorPeriodo(periodo.FormatarDataInicio(), periodo.FormatarDataFim(), token);
            return Ok(vendas);
        }


        [HttpGet("{id}", Name = "BuscarVendasPorId")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> BuscarVendasPorId(string id, CancellationToken token)
        {
            var vendas = await _service.BuscarVendasPorId(id, token);
            return Ok(vendas);
        }

        [HttpGet("Clientes/")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> BuscarClientes(CancellationToken token)
        {
            var clientes = await _service.BuscarClientes(token);
            return Ok(clientes);
        }

        [HttpGet("Produtos/")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> BuscarProdutos(CancellationToken token)
        {
            var produtos = await _service.BuscarProdutos(token);
            return Ok(produtos);
        }

        [HttpGet("FormasdePagamento/")]
        public ActionResult<IDictionary<int, string>> ListarFormasDePagamento(CancellationToken token)
        {
            return Ok(_service.ListarFormasDePagamento());
        }

        [HttpGet("StatusVenda/")]
        public ActionResult<IDictionary<int, string>> ListarStatus(CancellationToken token)
        {
            return Ok(_service.ListarStatusDeVenda());
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<VendaDto>>> CriarVenda(CriarVendaCommand venda, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

        [HttpPut]
        public async Task<ActionResult> AtualizarVenda(AtualizarVendaCommand venda, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sucesso = await _handler.Handle(venda, token);
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


        [HttpPut("Confirmar/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> ConfirmarVenda(ConfirmarVendaCommand venda, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sucesso = await _handler.Handle(venda, token);
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

        [HttpPut("Cancelar/")]
        public async Task<ActionResult> CancelarVenda(CancelarVendaCommand venda, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sucesso = await _handler.Handle(venda, token);
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

        [HttpPost("Item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> AdicionarItemEmVenda(AdicionarItemVendaCommand item, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sucesso = await _handler.Handle(item, token);
                    if (sucesso)
                    {
                        var vendas = await _service.BuscarVendasPorId(item.VendaId, token);
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

        [HttpPut("Item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> AtualizarItemEmVenda(AtualizarItemVendaCommand item, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

        [HttpDelete("Item/")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> RemoverItemDeVenda(RemoverItemVendaCommand item, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
