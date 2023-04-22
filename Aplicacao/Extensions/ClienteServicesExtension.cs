using Clientes.Application.Commands;
using Clientes.Application.Query;
using Clientes.Domain.Model;
using Clientes.Domain.Repository;
using Clientes.Domain;
using Clientes.Infrastructure;
using Core.Infrastructure;

namespace AplicacaoGerenciamentoLoja.Extensions
{
    public static class ClienteServicesExtension
    {
        public static IServiceCollection AddClienteServicesExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext<Cliente>, ClienteDbContext>(_ => new ClienteDbContext(configuration.GetConnectionString("ClientesConnectionString")));
            services.AddScoped<IUnitOfWork<Cliente>, UnitOfWork<Cliente>>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddScoped<ClienteCommandHandler>();
            services.AddScoped<ClienteQueryService>();
            services.Configure<ClienteDomainSettings>(configuration.GetSection("Queues").GetSection("ClienteDomainSettings"));

            return services;
        }
    }
}
