using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuyMyHouse.Infrastructure
{
    public interface ITableStorage<T> where T : TableEntity, new()
    {
        Task CreateNewTable();
        Task<T> CreateEntity(T entity);
        Task<T> GetEntityByPartitionKeyAndRowKey(string partitionKey, string rowKey);
        Task<T> UpdateEntity(T entity);
        Task<bool> DeleteEntity(string partitionKey, string rowKey);

    }
}
