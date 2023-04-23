using Clientes.Domain.Model;
using static Clientes.Domain.Model.Status;

namespace Clientes.Application.Query.DTO
{
    public record ClienteQueryDto(string Id, string Cpf, string Nome, string Email, ClienteStatus EstaAtivo, Endereco endereco);
    public record ClienteMutateDto(string Cpf, string Nome);
}
