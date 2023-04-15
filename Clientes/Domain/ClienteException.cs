using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clientes.Domain
{
    public class ClienteException : Exception
    {
        public ClienteException(string message)
        {
            Message = message;
        }
        public override string Message { get; }

    }
}
