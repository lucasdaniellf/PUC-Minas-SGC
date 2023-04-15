using Core.Entity;
using static Produtos.Domain.Model.LogEstoque;

namespace Produtos.Domain.Model
{
    public class Estoque : IEntity
    {
        public string Id { get; private set; } = null!;
        public int Quantidade { get; private set; } = 0;
        public int EstoqueMinimo { get; private set; } = 0;
        public DateTime UltimaAlteracao { get; private set; } 
        
        public void AtualizarEstoqueMinimo(int quantidade)
        {
            if (quantidade < 0)
            {
                throw new ProdutoException("Quantidade mínima deve ser maior ou igual a 0");
            }
            EstoqueMinimo = quantidade;
            VerificarQuantidadeEmEstoque(this.Quantidade);
        }

        public void AtualizarEstoque(int quantidade)
        {
            VerificarQuantidadeEmEstoque(quantidade);
            this.Quantidade = quantidade;
        }

        private void VerificarQuantidadeEmEstoque(int quantidade)
        {
            if(quantidade < 0)
            {
                throw new ProdutoException("Quantidade insuficiente em estoque");
            }
            if (quantidade < EstoqueMinimo)
            {
                //Dispara evento alertando baixa quantidade em estoque
            }
        }

        public static Estoque CriarEstoque()
        {
            return new Estoque()
            {
                Id = Guid.NewGuid().ToString(),
                UltimaAlteracao = DateTime.Now
            };
        }

        private Estoque()
        {

        }

        public Estoque(string id, int quantidade, int minimaQuantidade, DateTime UltimaAlteracao)
        {
            Id = id;
            this.UltimaAlteracao = UltimaAlteracao;
            AtualizarEstoque(quantidade);
            AtualizarEstoqueMinimo(minimaQuantidade);
        }

        public LogEstoque GerarLog()
        {

            return new LogEstoque()
            {
                EstoqueId = this.Id,
                HorarioAtualizacao = this.UltimaAlteracao,
                Quantidade = this.Quantidade,
            };
        }

    }
}