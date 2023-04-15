using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produtos.Application.Commands.ProdutoEstoque
{
    public class EstoqueProduto
    {
        public string ProdutoId { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Quantidade { get; set; }
    }
}
