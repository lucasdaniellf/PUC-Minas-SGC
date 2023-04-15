using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produtos.Application.Commands
{
    public class CadastrarProdutoCommand : CommandRequest
    {
        public string Id { get; internal set; } = string.Empty;
        public string Descricao { get; set; } = null!;
        public decimal Preco { get; set; }
    }
}
