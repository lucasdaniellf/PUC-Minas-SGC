using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Model;

namespace Vendas.Application.Commands
{
    public class AtualizarItemVendaCommand : CommandRequest
    {
        public string VendaId { get; set; } = null!;
        public string ProdutoId { get; set; } = null!;
        public int Quantidade { get; set; }
    }
}
