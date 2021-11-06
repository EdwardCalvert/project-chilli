using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace DataLibrary
{
    public class MySqlDataAccess : IDataAccess
    {
        public async Task<List<U>> LoadData<U>(string sql, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<U>(sql);

                return rows.ToList();
            }

        }

        public Task SaveData(string sql, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                return connection.ExecuteAsync(sql);

            }

        }
    }
}
