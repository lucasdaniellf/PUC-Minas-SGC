using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Produtos.Domain.Model.Status;

namespace Produtos.Application.Commands
{
    public class AtualizarCadastroProdutoCommand : CommandRequest
    {
        public string Id { get; private set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal Preco { get; set; }
        public int EstoqueMinimo { get; set; }
        public ProdutoStatus EstaAtivo { get; set; }
        public void AdicionarId(string id)
        {
            Id = id;
        }
    }
}
