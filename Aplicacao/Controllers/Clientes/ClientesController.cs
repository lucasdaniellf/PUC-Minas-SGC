using Clientes.Application.Commands;
using Clientes.Application.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AplicacaoGerenciamentoLoja.Controllers.Clientes
{
    [Route("api/clientes")]
    [ApiController]
    public partial class ClientesController : ControllerBase
    {
        private readonly ClienteCommandHandler _handler;
        private readonly ClienteQueryService _service;

        public ClientesController(ClienteCommandHandler handler, ClienteQueryService service)
        {
            _handler = handler;
            _service = service;
        }
    }
}
