using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produtos.Application.Commands
{
    public class AdicionarProdutoEmCatalogoCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
    }
}
