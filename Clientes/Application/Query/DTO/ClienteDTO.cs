using Clientes.Domain.Model;
using static Clientes.Domain.Model.ClienteStatus;

namespace Clientes.Application.Query.DTO
{
    public record ClienteQueryDto(string Id, string Cpf, string Nome, string Email, ClienteStatusEnum Status, Endereco Endereco);
    public record ClienteMutateDto(string Cpf, string Nome);
}
