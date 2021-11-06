using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibrary
{
    public interface IDataAccess
    {
        Task<List<U>> LoadData<U>(string sql, string connectionString);
        Task SaveData(string sql, string connectionString);
    }
}