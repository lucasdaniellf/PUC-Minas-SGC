using Core.Infrastructure;
using Produtos.Application.Commands;
using Produtos.Application.Query;
using Produtos.Domain.Model;
using Produtos.Domain.Repository;
using Produtos.Domain;
using Produtos.Infrastructure;

namespace AplicacaoGerenciamentoLoja.Extensions
{
    public static class ProtutosServicesExtension
    {
        public static IServiceCollection AddProdutosServicesExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext<Produto>, ProdutoDbContext>(_ => new ProdutoDbContext(configuration.GetConnectionString("ProdutosConnectionString")));
            services.AddScoped<IUnitOfWork<Produto>, UnitOfWork<Produto>>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            services.AddScoped<ProdutoCommandHandler>();
            services.AddScoped<ProdutoQueryService>();
            services.Configure<ProdutoDomainSettings>(configuration.GetSection("Queues").GetSection("ProdutoDomainSettings"));

            return services;
        }
    }
}
