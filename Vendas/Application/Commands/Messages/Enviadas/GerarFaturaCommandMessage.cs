using Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Messages.Enviadas
{
    public class GerarFaturaCommandMessage : MessageRequest
    {
        public GerarFaturaCommandMessage(string VendaId)
        {
            this.VendaId = VendaId;
        }

        public string VendaId { get; private set; } = null!;

    }
}
