namespace AplicacaoGerenciamentoLoja.SystemPolicies
{
    public class Policies
    {
        public const string RequisitoCadastrarVenda = nameof(RequisitoCadastrarVenda);
        public const string RequisitoAtualizarVenda = nameof(RequisitoAtualizarVenda);
        public const string RequisitoLerDadosVenda = nameof(RequisitoLerDadosVenda);

        public const string RequisitoGerenciamentoProduto = nameof(RequisitoGerenciamentoProduto);

        public const string RequisitoCadastroCliente = nameof(RequisitoCadastroCliente);
        public const string RequisitoAtualizarCliente = nameof(RequisitoAtualizarCliente);
        public const string RequisitoLerDadosCliente = nameof(RequisitoLerDadosCliente);
        public const string RequisitoApenasAcessoInterno = nameof(RequisitoApenasAcessoInterno);
    }
}
