using Core.Messages.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Produtos.Domain.Model.Status;

namespace Produtos.Application.Events
{
    public class ProdutoMensagemEvent : EventRequest
    {
        public ProdutoMensagemEvent(string id, decimal preco, int quantidade, int estaAtivo)
        {
            Id = id;
            Preco = preco;
            Quantidade = quantidade;
            EstaAtivo = estaAtivo;
        }

        public string Id { get; private set; } = null!;
        public decimal Preco { get; private set; }
        public int Quantidade { get; private set; }
        public int EstaAtivo { get; private set; }
    }
}
