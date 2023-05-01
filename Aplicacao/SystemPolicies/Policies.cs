namespace AplicacaoGerenciamentoLoja.SystemPolicies
{
    public static class Policies
    {
        public const string PoliticaCriarCadastroCliente = nameof(PoliticaCriarCadastroCliente);
        public const string PoliticaAtualizarCadastroCliente = nameof(PoliticaAtualizarCadastroCliente);
        public const string PoliticaLerCadastroCliente = nameof(PoliticaLerCadastroCliente);

        public const string PoliticaGerenciamentoProduto = nameof(PoliticaGerenciamentoProduto);

        public const string PoliticaCriarVenda = nameof(PoliticaCriarVenda);
        public const string PoliticaAtualizarVenda = nameof(PoliticaAtualizarVenda);
        public const string PoliticaLerVenda = nameof(PoliticaLerVenda);

        public const string PoliticaAcessoInterno = nameof(PoliticaAcessoInterno);
        public const string PoliticaValidacaoEmailUsuario = nameof(PoliticaValidacaoEmailUsuario);
    }
}
