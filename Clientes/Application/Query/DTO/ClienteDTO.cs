using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Query.DTO
{
    public record ClienteQueryDto(string Id, string Cpf, string Nome, ClienteStatus EstaAtivo);
    public record ClienteMutateDto(string Cpf, string Nome);
}
