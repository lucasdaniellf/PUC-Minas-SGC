using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class ReporEstoqueProdutoCommand : CommandRequest
    {
        public IEnumerable<EstoqueProduto> Produtos { get; set; } = Enumerable.Empty<EstoqueProduto>();
    }
}
