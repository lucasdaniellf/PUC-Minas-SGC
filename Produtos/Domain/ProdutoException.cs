using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produtos.Domain
{
    public class ProdutoException : Exception
    {
        public ProdutoException(string message)
        {
            Message = message;
        }
        public override string Message { get; }

    }
}
