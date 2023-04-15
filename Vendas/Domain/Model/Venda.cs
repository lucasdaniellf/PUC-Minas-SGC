using Core.Entity;
using static Vendas.Domain.Model.FormaPagamentoEnum;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Domain.Model
{
    public class Venda : IAggregateRoot
    {
        internal string Id { get; private set; } = null!;
        internal DateTime DataVenda { get; private set; } = DateTime.Now;
        internal int Desconto { get; private set; }
        internal Status Status { get; private set; } = Status.PENDENTE;
        internal FormaPagamento FormaDePagamento { get; private set; } = FormaPagamento.A_VISTA;
        internal ClienteVenda Cliente { get; private set; } = null!;
        internal IList<ItemVenda> Items { get; private set; }

        internal Venda(string Id, ClienteVenda cliente, DateTime DataVenda, int Desconto, int FormaPagamento, int Status) : this(cliente)
        {
            this.Id = Id;
            this.DataVenda = DataVenda;
            this.Desconto = Desconto;
            this.Status = AplicarStatus(Status);
            this.FormaDePagamento = SelecionarFormaDePagamento(FormaPagamento);
        }

        private Venda(ClienteVenda cliente)
        {
            this.Cliente = cliente;
            Items = new List<ItemVenda>();
        }
        internal static Venda CriarVenda(ClienteVenda cliente)
        {
            if (cliente.Status == ClienteVenda.ClienteStatus.INATIVO)
            {
                throw new VendaException("Cliente com status inativo");
            }

            var venda = new Venda(cliente)
            {
                Id = Guid.NewGuid().ToString()
            };

            return venda;
        }
        internal void AdicionarItemAVenda(ItemVenda item)
        {
            if (Status != Status.PENDENTE && Status != Status.REPROVADO)
            {
                throw new VendaException("Venda não pode ser alterada. Status: " + Status);
            }
            if(this.Items.Where( x => x.Produto.Id == item.Produto.Id).ToList().Any())
            {
                throw new VendaException("Produto já se encontra nesta venda");
            }
            this.Items.Add(item);
        }
        internal IEnumerable<ItemVenda> RemoverItemVenda(string produtoId)
        {
            if (Status != Status.PENDENTE && Status != Status.REPROVADO)
            {
                throw new VendaException("Venda não pode ser alterada. Status: " + Status);
            }
            var item = Items.Where(x => x.Produto.Id == produtoId).ToList();
            if (item.Any())
            {
                Items.Remove(item.First());
            }
            return item;
        }

        internal void AtualizarDadosVenda(int desconto, FormaPagamento formaPagamento )
        {
            if (Status != Status.PENDENTE && Status != Status.REPROVADO)
            {
                throw new VendaException("Venda não pode ser alterada. Status: " + Status);
            }

            AplicarDesconto(desconto);
            AtualizarFormaDePagamento(formaPagamento);
        }

        internal void ProcessarVenda()
        {
            if (Status != Status.PENDENTE && Status != Status.REPROVADO)
            {
                throw new VendaException("Venda não pode ser processada. Status: " + Status);
            }
            if (!Items.Any()) 
            {
                throw new VendaException("Venda não pode ser processada sem items");
            }
            if (Cliente.Status == ClienteVenda.ClienteStatus.INATIVO)
            {
                throw new VendaException("Venda não pode ser processada para cliente com status inativo. Cliente: "+ Cliente.Id);
            }
            foreach(var item in Items)
            {
                if (!item.ValidarProdutoItemVenda())
                {
                    throw new VendaException($"Venda não pode ser processada devido à produto {item.Produto.Id} - Status: {item.Produto.EstaAtivo}; Quantidade: {item.Produto.QuantidadeEstoque}");
                }
            }
            Status = Status.PROCESSANDO;
        }

        internal void CancelarVenda()
        {
            //if (Status != Status.PENDENTE && Status != Status.REPROVADO)
            //{
            //    throw new VendaException("Venda não pode ser cancelada. Status: " + Status);
            //}
            //Status = Status.CANCELADO;


            if (Status == Status.PROCESSANDO || Status == Status.CANCELADO)
            {
                throw new VendaException("Venda não pode ser cancelada. Status: " + Status);
            }
            if (Status == Status.APROVADO && ((DataVenda - DateTime.Now).Days > 2))
            {
                throw new VendaException($"Venda não pode ser cancelada, prazo para cancelamento extrapolado: {(DataVenda - DateTime.Now).Days}");
            }
            Status = Status.CANCELADO;
        }
        internal void FinalizarVenda()
        {
            if (Status != Status.AGUARDANDO_PAGAMENTO)
            {
                throw new VendaException("Venda não pode ser Finalizada. Status: " + Status);
            }
            DataVenda = DateTime.Now;
            Status = Status.APROVADO;
        }
        internal void ReprovarVenda()
        {
            if (Status != Status.PROCESSANDO && Status != Status.AGUARDANDO_PAGAMENTO)
            {
                throw new VendaException("Venda não pode ser Reprovada. Status: " + Status);
            }
            Status = Status.REPROVADO;
        }

        internal void FaturarVenda()
        {
            if (Status != Status.PROCESSANDO)
            {
                throw new VendaException("Venda não pode ser Faturada. Status: " + Status);
            }
            Status = Status.AGUARDANDO_PAGAMENTO;
        }

        private void AtualizarFormaDePagamento(FormaPagamento formaPagamento)
        {
            var success = Enum.IsDefined(typeof(FormaPagamento), formaPagamento);
            if (success)
            {
                this.FormaDePagamento = formaPagamento;
            }
            else
            {
                throw new VendaException("Forma de Pagamento Inválida");
            }
        }

        private void AplicarDesconto(int desconto)
        {
            if (desconto < 0 || desconto > 100)
            {
                throw new VendaException("Desconto deve estar entre 0% e 100%.");
            }
            Desconto = desconto;
            //foreach (var item in Items)
            //{
            //    item.AtualizarValorPago((1 - desconto) * item.ValorPago); ;
            //}
        }
    }
}
