using Clientes.Domain.Model;
using static Clientes.Infrastructure.ClienteDataObject;

namespace Clientes.Infrastructure
{
    internal class ClienteDataObject
    {
        internal record ClienteTO(string Id, string Cpf, string Nome, string Email, long Status, string Rua, string NumeroCasa, string? Complemento, string CEP, string Bairro, string Cidade, string Estado);
    }

    internal class ClienteRepositoryMapping
    {
        internal static Cliente MapearClienteEndereco(ClienteTO to)
        {
            return new Cliente(to.Id, to.Cpf, to.Nome, to.Email, to.Status, new Endereco(to.Rua, to.NumeroCasa, to.Complemento, to.CEP, to.Cidade, to.Bairro, to.Estado));
        }
    }
}
