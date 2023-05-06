using Dapper;
using Vendas.Domain.Model;
using Vendas.Infrastructure;
using static Vendas.Domain.Model.StatusVenda;

namespace Vendas.Domain.Repository
{
    public interface IVendaRepository
    {
         Task<IEnumerable<Venda>> BuscarVendas(CancellationToken token);
         Task<IEnumerable<Venda>> BuscarVendaPorId(string id, CancellationToken token);
         Task<IEnumerable<Venda>> BuscarVendasPorCliente(string clienteId, CancellationToken token);
         Task<IEnumerable<Venda>> BuscarVendasPorData(DateTime dataInicio, DateTime dataFim, CancellationToken token);
         Task<IEnumerable<Venda>> BuscarVendaPorStatusVenda(Status status, CancellationToken token);
         Task<int> CadastrarVenda(Venda venda, CancellationToken token);
         Task<int> AtualizarVenda(Venda venda, CancellationToken token);
         Task<int> AdicionarProdutoEmVenda(ItemVenda item, CancellationToken token);
         Task<int> RemoverProdutoEmVenda(ItemVenda item, CancellationToken token);
         Task<int> AtualizarProdutoEmVenda(ItemVenda item, CancellationToken token);
        //=====================Produto=====================//
         Task<IEnumerable<ProdutoVenda>> BuscarProdutos(CancellationToken token);
         Task<IEnumerable<ProdutoVenda>> BuscarProdutoPorId(string id, CancellationToken token);
         Task<int> CadastrarProduto(ProdutoVenda produto, CancellationToken token);
         Task<int> AtualizarCadastroProduto(ProdutoVenda produto, CancellationToken token);
        //=================Cliente====================//
         Task<IEnumerable<ClienteVenda>> BuscarClientes(CancellationToken token);
         Task<IEnumerable<ClienteVenda>> BuscarClientePorId(string id, CancellationToken token);
         Task<IEnumerable<ClienteVenda>> BuscarClientePorEmail(string email, CancellationToken token);

         Task<int> AtualizarCliente(ClienteVenda cliente, CancellationToken token);
         Task<int> CadastrarCliente(ClienteVenda cliente, CancellationToken token);
    }
}
