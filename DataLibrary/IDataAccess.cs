using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibrary
{
    public interface IDataAccess
    {
        Task<List<U>> LoadData<U, T>(string sql, T Parameters, string connectionString);
        Task SaveData<T>(string sql, T Parameters, string connectionString);
    }
}
