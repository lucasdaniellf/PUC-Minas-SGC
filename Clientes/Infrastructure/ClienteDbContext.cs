using Clientes.Domain.Model;
using Core.Infrastructure;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;


namespace Clientes.Infrastructure
{
    public class ClienteDbContext : IDbContext<Cliente>
    {
        public ClienteDbContext(string conn)
        {
            //Development
            string fileLocation = string.Concat(Path.GetFullPath("."), "\\db\\ClientesDb");

            //string fileLocation = string.Concat(Path.GetFullPath("."), "/Aplicacao/db/ClientesDb");

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
                        Id varchar PRIMARY KEY,
                        Cpf varchar(11) not null unique,
                        Nome varchar(100) not null,
                        Email varchar(100) not null unique,
                        EstaAtivo INTEGER not null,
                        check(EstaAtivo = 0 OR EstaAtivo = 1)
                    );                        
                ";
                conn.Execute(sql);

                conn.Dispose();
            }
        }
    }
}
