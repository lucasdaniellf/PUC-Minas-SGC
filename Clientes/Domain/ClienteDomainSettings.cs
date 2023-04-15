using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clientes.Domain
{
    public class ClienteDomainSettings
    {
        public string FilaClienteAtualizado { get; set; } = null!;
        public string FilaClienteCadastrado { get; set; } = null!;
        public string FilaClienteStatusAlterado { get; set; } = null!;

    }
}
