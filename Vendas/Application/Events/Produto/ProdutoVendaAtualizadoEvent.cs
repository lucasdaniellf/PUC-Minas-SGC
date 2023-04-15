using Core.Messages.Event;

namespace Vendas.Application.Events.Produto
{
    public class ProdutoVendaAtualizadoEvent : EventRequest
    {
        public ProdutoVendaAtualizadoEvent(string id, decimal preco, int quantidade, int estaAtivo)
        {
            Id = id;
            Preco = preco;
            QuantidadeEstoque = quantidade;
            EstaAtivo = estaAtivo;
        }

        public string Id { get; private set; } = null!;
        public decimal Preco { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public int EstaAtivo { get; private set; }
    }
}
