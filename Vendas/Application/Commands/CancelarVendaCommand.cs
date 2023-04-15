using Core.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands
{
    public class CancelarVendaCommand : CommandRequest
    {
        public string Id { get; set; } = null!;
    }
}
