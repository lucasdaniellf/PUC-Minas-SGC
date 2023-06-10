using Core.Messages.Commands;
using System.ComponentModel.DataAnnotations;

namespace Produtos.Application.Commands
{
    public class AtualizarEstoqueProdutoCommand : CommandRequest
    {
        public string ProdutoId { get; private set; } = null!;

        [Range(0, int.MaxValue)]
        public int EstoqueMinimo { get; private set; }

        [Range(0, int.MaxValue)]
        public int QuantidadeAtual { get; private set; }

        public AtualizarEstoqueProdutoCommand(string produtoId, int estoqueMinimo, int quantidadeAtual)
        {
            ProdutoId = produtoId;
            EstoqueMinimo = estoqueMinimo;
            QuantidadeAtual = quantidadeAtual;
        }
    }
}
