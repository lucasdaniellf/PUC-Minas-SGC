using Core.Entity;

namespace Produtos.Domain.Model
{
    public class LogEstoque : IEntity
    {
        public int Id { get; internal set; }
        public string EstoqueId { get; internal set; } = null!;
        public DateTime HorarioAtualizacao { get; internal set; }
        public int Quantidade { get; internal set; }

        internal LogEstoque() { }
    }
}
