using Core.Infrastructure;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using Vendas.Domain.Model;

namespace Vendas.Infrastructure
{
    public class VendaDbContext : IDbContext<Venda>
    {
        public VendaDbContext(string conn)
        {
            //Development
            //string fileLocation = string.Concat(Path.GetFullPath("."), "\\db\\VendasDb");

            string fileLocation = string.Concat(Path.GetFullPath("."), "/Aplicacao/db/VendasDb");
            conn = conn.Replace("{AppDir}", fileLocation);

            if (!File.Exists(fileLocation))
            {
                CreateDatabase(conn);
            }
            Connection = CreateConnection(conn);
        }
        public IDbConnection Connection { get; private set; }

        public IDbTransaction? Transaction { get; set; }

        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        private void CreateDatabase(string connectionString)
        {
            //File.Create(string.Concat(Path.GetFullPath("."), "\\Repository\\db\\database.db"));
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    Create table Cliente(
                        Id varchar Primary key,
                        Email Varchar(100) unique not null,
                        Status INTEGER not null
                    );
                    
                    Create table Venda(
                        Id varchar PRIMARY KEY,
                        ClienteId varchar not null,
                        DataVenda datetime not null,
                        Desconto INTEGER not null,
                        FormaPagamento INTEGER not null,
                        Status INTEGER not null,
                        CriadoPor varchar not null,
                        check((FormaPagamento = 0 OR FormaPagamento = 1) AND (Status in (0,1,2,3,4,99))),
                        FOREIGN KEY(ClienteId) References Cliente(Id) ON DELETE SET NULL
                    );

                    Create table Produto(
                        Id varchar PRIMARY KEY,
                        Preco real not null,
                        QuantidadeEstoque INTEGER not null,
                        Status INTEGER not null
                    );

                    Create table ItemVenda(
                        VendaId string not null,
                        ProdutoId string not null,
                        ValorPago real not null,
                        Quantidade int not null,
                        check(Quantidade > 0 AND ValorPago >= 0)
                        FOREIGN KEY(ProdutoId) references Produto(Id),
                        FOREIGN KEY(VendaId) references Venda(Id),
                        PRIMARY KEY(ProdutoId, VendaId)
                    );
                ";
                conn.Execute(sql);

                conn.Dispose();
            }
        }
    }
}
