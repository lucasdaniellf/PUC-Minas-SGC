﻿using Core.Entity;
using Produtos.Infrastructure;
using static Produtos.Domain.Model.ProdutoStatus;

namespace Produtos.Domain.Model
{
    public class Produto : IAggregateRoot
    {
        public string Id { get; private set; } = null!;
        public string Descricao { get; private set; } = null!;
        public decimal Preco { get; private set; }
        public Estoque Estoque { get; private set; } = null!;
        public ProdutoStatusEnum Status { get; private set; } = ProdutoStatusEnum.ATIVO;

        public void AtualizarStatusProduto(ProdutoStatusEnum status)
        {
            if(!Enum.IsDefined(typeof(ProdutoStatusEnum), status))
            {
                throw new ProdutoException("Status de produto inválido, deve ser 0 (inativo) ou 1 (ativo)");
            }
            Status = status;
        }

        public void AtualizarDescricao(string descricao)
        {
            if (string.IsNullOrEmpty(descricao))
            {
                throw new ProdutoException("Descricao inválido, não deve ser vazio");
            }
            Descricao = descricao;
        }

        public void AtualizarPreco(decimal preco)
        {
            if (preco <= 0)
            {
                throw new ProdutoException("Preço inválido, deve ser maior que 0");
            }
            Preco = preco;
        }

        public void BaixarEstoque(int quantidade)
        {
            if (Status == ProdutoStatusEnum.INATIVO)
            {
                throw new ProdutoException("Produto se encontra inativo");
            }
            this.Estoque.AtualizarEstoque(this.Estoque.Quantidade - quantidade);
        }
        //Caso se realize o cancelamento de uma venda cujo produto foi desativado, se realizássemos a validação do status do produto, a venda seria cancelada mas o estoque não seria reposto.
        public void ReporEstoque(int quantidade)
        {
            this.Estoque.AtualizarEstoque(this.Estoque.Quantidade + quantidade);
        }

        public static Produto CadastrarProduto(string descricao, decimal preco)
        {
            Produto produto = new(descricao, preco)
            {
                Id = Guid.NewGuid().ToString(),
                Estoque = Estoque.CriarEstoque()
            };
            return produto;
        }



        internal Produto(string id, string descricao, decimal preco, long status, string estoqueId, int quantidade, int estoqueMinimo, DateTime UltimaAlteracao) : this(descricao, preco)
        {
            Id = id;
            Estoque = new Estoque(estoqueId, quantidade, estoqueMinimo, UltimaAlteracao);
            Status = AplicarStatusEmProduto(status);
        }

        private Produto(string descricao, decimal preco)
        {
            AtualizarDescricao(descricao);
            AtualizarPreco(preco);
        }
    }
}
