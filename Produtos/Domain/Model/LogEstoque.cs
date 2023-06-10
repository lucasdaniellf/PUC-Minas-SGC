using Core.Entity;

namespace Produtos.Domain.Model
{
    public class LogEstoque : IEntity
    {
        public int Id { get; private set; }
        public string EstoqueId { get; private set; } = null!;
        public DateTime HorarioAtualizacao { get; private set; }
        public int Quantidade { get; private set; }

        internal LogEstoque(string EstoqueId, DateTime HorarioAtualizacao, int Quantidade) 
        {
            this.EstoqueId = EstoqueId;
            this.HorarioAtualizacao = HorarioAtualizacao;
            this.Quantidade = Quantidade;
        }

        private LogEstoque() { }
    }
}
