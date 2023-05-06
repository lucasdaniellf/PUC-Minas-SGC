using Core.Infrastructure;
using Vendas.Application.Commands.Handlers;
using Vendas.Application.Events;
using Vendas.Application.Query;
using Vendas.Domain.Repository;
using Vendas.Domain;
using Vendas.Infrastructure;
using Vendas.Domain.Model;

namespace AplicacaoGerenciamentoLoja.Extensions
{
    public static class VendaServicesExtension
    {
        public static IServiceCollection AddVendasServicesExtension(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddScoped<IDbContext<Venda>, VendaDbContext>(_ => new VendaDbContext(configuration.GetConnectionString("VendasConnectionString")));
            services.AddScoped<IUnitOfWork<Venda>, UnitOfWork<Venda>>();
            services.AddScoped<IVendaRepository, VendaRepository>();

            services.AddScoped<VendaCommandHandler>();
            services.AddScoped<VendaQueryService>();
            services.AddScoped<VendaEventHandler>();
            services.Configure<VendaDomainSettings>(configuration.GetSection("Queues").GetSection("VendaDomainSettings"));
            


            return services;
        }
    }
}
