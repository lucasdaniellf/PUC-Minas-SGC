using AplicacaoGerenciamentoLoja.HostedServices.Consumers.Cliente;
using AplicacaoGerenciamentoLoja.HostedServices.Consumers.Faturamento;
using AplicacaoGerenciamentoLoja.HostedServices.Consumers.Produto;
using AplicacaoGerenciamentoLoja.HostedServices.Consumers.Venda;

namespace AplicacaoGerenciamentoLoja.Extensions
{
    public static class HostedServiceExtensions
    {
        public static IServiceCollection AddHostedServicesExtension(this IServiceCollection services) 
        {
            services.AddHostedService<ClienteAtualizadoConsumer>();
            services.AddHostedService<ProdutoAtualizadoConsumer>();

            services.AddHostedService<FaturarVendaConsumer>();
            services.AddHostedService<FaturamentoCallbackConsumer>();
            services.AddHostedService<ReservarProdutoConsumer>();

            services.AddHostedService<ReporProdutoConsumer>();

            return services;
        }

    }
}
