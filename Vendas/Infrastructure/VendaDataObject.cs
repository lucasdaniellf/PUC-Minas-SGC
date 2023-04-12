using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Model;

namespace Vendas.Infrastructure
{
    internal record VendaTO(string Id,
                            string DataVenda,
                            long Desconto,
                            long FormaPagamento,
                            long Status,
                            string ClienteId,
                            long ClienteStatus
                            )
    {
        internal IList<ItemVendaTO> itens = new List<ItemVendaTO>();
    }

    internal record ItemVendaTO(string VendaId,
                                long Quantidade,
                                double ValorPago,
                                string ProdutoId,
                                double PrecoProduto,
                                long QuantidadeEstoque,
                                long ProdutoStatus);

    internal class VendaDataObject
    {
        internal static Venda MapearVendaDO(VendaTO TO)
        {
            
            var venda = new Venda(TO.Id, new ClienteVenda(TO.ClienteId, (int) TO.ClienteStatus), Convert.ToDateTime(TO.DataVenda), (int)TO.Desconto, (int)TO.FormaPagamento, (int)TO.Status);
            foreach(var item in TO.itens)
            {
                venda.Items.Add(MapearItemVendaTO(venda, item));
            }

            return venda;
        }

        private static ItemVenda MapearItemVendaTO(Venda venda, ItemVendaTO TO)
        {
            return new ItemVenda(venda, new ProdutoVenda(TO.ProdutoId, (decimal)TO.PrecoProduto, (int)TO.QuantidadeEstoque, (int)TO.ProdutoStatus), (int)TO.Quantidade, (decimal)TO.ValorPago);
        }
    }
}
