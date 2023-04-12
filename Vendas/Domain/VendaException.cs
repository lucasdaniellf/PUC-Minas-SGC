using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain
{
    public class VendaException : Exception
    {
        public VendaException(string erro) 
        {
            Message = erro;        
        }

        public override string Message { get; } = null!;
    }
}
