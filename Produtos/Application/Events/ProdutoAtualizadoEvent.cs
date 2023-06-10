using Core.Messages.Event;
using static Produtos.Domain.Model.ProdutoStatus;

namespace Produtos.Application.Events
{
    public class ProdutoAtualizadoEvent : EventRequest
    {
        public ProdutoAtualizadoEvent(string id, decimal preco, int quantidade, ProdutoStatusEnum status)
        {
            Id = id;
            Preco = preco;
            Quantidade = quantidade;
            Status = status;
        }

        public string Id { get; private set; } = null!;
        public decimal Preco { get; private set; }
        public int Quantidade { get; private set; }
        public ProdutoStatusEnum Status { get; private set; }
    }
}
