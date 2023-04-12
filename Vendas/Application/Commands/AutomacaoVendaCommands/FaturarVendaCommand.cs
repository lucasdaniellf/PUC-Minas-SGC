using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.AutomacaoVendaCommands
{
    public class FaturarVendaCommand : CommandRequest
    {
        public FaturarVendaCommand(string vendaId)
        {
            VendaId = vendaId;
        }

        public string VendaId { get; set; } = null!;
    }
}
