using Core.Infrastructure;
using Microsoft.Data.Sqlite;
using Produtos.Domain.Model;
using System.Data;
using Dapper;

namespace Produtos.Infrastructure
{
    public class ProdutoDbContext : IDbContext<Produto>
    {
        public ProdutoDbContext(string conn)
        {
            //Development
            //string fileLocation = string.Concat(Path.GetFullPath("."), "\\db\\ProdutosDb");
            string fileLocation = string.Concat(Path.GetFullPath("."), "/Aplicacao/db/ProdutosDb");
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
                    Create table Produto(
                        Id varchar PRIMARY KEY,
                        Descricao varchar(300) not null unique,
                        Preco real not null,
                        EstaAtivo INTEGER not null,
                        check(Preco >= 0 AND (EstaAtivo = 1 OR EstaAtivo = 0))
                    );


                    Create table Estoque(
                        Id string PRIMARY KEY,
                        ProdutoId varchar not null unique,
                        Quantidade INTEGER not null,
                        EstoqueMinimo INTEGER not null,
                        UltimaAlteracao datetime not null,
                        check(Quantidade >= 0 AND EstoqueMinimo >= 0),
                        FOREIGN KEY(ProdutoId) REFERENCES Produto(Id) ON DELETE CASCADE
                    );
                    
                    Create Table LogEstoque(
                        Id integer PRIMARY KEY autoincrement,
                        EstoqueId varchar not null,
                        HorarioAlteracao datetime not null,
                        Quantidade integer not null,
                        Foreign key(EstoqueId) References Estoque(Id) on delete cascade                        
                    );
                ";
                conn.Execute(sql);

                conn.Dispose();
            }
        }
    }
}
