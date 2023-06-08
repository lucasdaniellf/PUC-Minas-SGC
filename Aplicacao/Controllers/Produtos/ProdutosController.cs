using AplicacaoGerenciamentoLoja.Controllers.Produtos.Parametros;
using AplicacaoGerenciamentoLoja.SystemPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Produtos.Application.Commands;
using Produtos.Application.Commands.ProdutoEstoque;
using Produtos.Application.Query;
using Produtos.Application.Query.DTO;
using Produtos.Domain;

namespace AplicacaoGerenciamentoLoja.Controllers.Produtos
{
    [Authorize(Roles = Roles.GerenteProdutos)]
    [Route("api/produtos")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoCommandHandler _handler;
        private readonly ProdutoQueryService _service;
        public ProdutosController(ProdutoQueryService service, ProdutoCommandHandler handler)
        {
            _service = service;
            _handler = handler;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProdutoQueryDto>>> BuscarProdutos(string? descricao, CancellationToken token)
        {

            IEnumerable<ProdutoQueryDto> Produtos;
            if (string.IsNullOrEmpty(descricao))
            {
                Produtos = await _service.BuscarProdutos(token);
            }
            else
            {
                Produtos = await _service.BuscarProdutoPorDescricao(descricao, token);
            }
            return Ok(Produtos);
        }

        [HttpGet("{Id}", Name = "BuscarProdutoPorId")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProdutoQueryDto>>> BuscarProdutoPorId(string Id, CancellationToken token)
        {

            IEnumerable<ProdutoQueryDto> Produtos = await _service.BuscarProdutoPorId(Id, token);
            if (Produtos.Any())
            {
                return Ok(Produtos);
            }

            return NotFound();
        }

        //Check this
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProdutoQueryDto>>> CadastrarProduto([FromBody] CadastrarProdutoRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new CadastrarProdutoCommand(request.Descricao, request.Preco);

                    var success = await _handler.Handle(command, token);
                    if (success)
                    {
                        var produto = await _service.BuscarProdutoPorId(command.Id, token);
                        return CreatedAtAction(nameof(BuscarProdutoPorId), new { produto.First().Id }, produto.First());
                    }
                }
                catch (ProdutoException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult> AtualizarCadastroProduto(string Id, [FromBody] AtualizarProdutoRequest request, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new AtualizarCadastroProdutoCommand(Id, request.Descricao, request.Preco, request.EstoqueMinimo, request.Status);

                    bool success = await _handler.Handle(command, token);
                    if (!success)
                    {
                        return NotFound();
                    }
                    return NoContent();
                }
                catch (ProdutoException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPatch("/estoque/alterar")]
        public async Task<ActionResult> AtualizarEstoqueProduto(string ProdutoId, int Quantidade, CancellationToken token)
        {
            try
            {
                var success = false;

                if(Quantidade > 0)
                {
                    IList<EstoqueProduto> produtos = new List<EstoqueProduto>
                    {
                        new EstoqueProduto(ProdutoId, Quantidade)
                    };

                    var command = new ReporEstoqueProdutoCommand(produtos);
                    success = await _handler.Handle(command, token);

                }
                else
                {
                    IList<EstoqueProduto> produtos = new List<EstoqueProduto>
                    {
                        new EstoqueProduto(ProdutoId, Math.Abs(Quantidade))
                    };

                    var command = new BaixarEstoqueProdutoCommand(produtos);
                    success = await _handler.Handle(command, token);
                }

                if (success)
                {
                    return NoContent();
                }
            }
            catch (ProdutoException ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound();
        }

        //[HttpPatch("{estoque/reposicao")]
        //public async Task<ActionResult> ReporEstoqueProduto(ReporEstoqueProdutoCommand command, CancellationToken token)
        //{
        //    try
        //    {
        //        bool success = await _handler.Handle(command, token);
        //        if (success)
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (ProdutoException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    return NotFound();
        //}


        //[HttpPatch("estoque/baixa")]
        //public async Task<ActionResult> BaixarEstoqueProduto(BaixarEstoqueProdutoCommand command, CancellationToken token)
        //{
        //    try
        //    {
        //        bool success = await _handler.Handle(command, token);
        //        if (success)
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (ProdutoException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    return NotFound();
        //}
    }
}
