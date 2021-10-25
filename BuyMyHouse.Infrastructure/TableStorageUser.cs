using BuyMyHouse.Domain;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyMyHouse.Infrastructure
{
    public class TableStorageUser : ITableStorage<UserDAL>
    {
        public CloudTable _cloudTable { get; set; }
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public async Task CreateNewTable()
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                string tableName = "Users";
                _cloudTable = tableClient.GetTableReference(tableName);
                var result = await _cloudTable.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public async Task<UserDAL> CreateEntity(UserDAL userEntity)
        {
            UserDAL userDAL = new UserDAL();
            try
            {
                TableOperation tableOperation = TableOperation.Insert(userEntity);
                TableResult result = await _cloudTable.ExecuteAsync(tableOperation);
                userDAL = result.Result as UserDAL;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }
            return userDAL;
        }

        public async Task<UserDAL> GetEntityByPartitionKeyAndRowKey(string partitionKey, string rowKey)
        {
            await RunDemo();
            try
            {
                TableQuery<UserDAL> query = new TableQuery<UserDAL>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)
                        )
                    );

                var userDAL = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);

                return userDAL.Results.FirstOrDefault();

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<UserDAL> UpdateEntity(UserDAL entity)
        {
           TableOperation tableOperation = TableOperation.Replace(entity);
           await _cloudTable.ExecuteAsync(tableOperation);
           return entity;
        }

        public async Task<bool> DeleteEntity(string partitionKey, string rowKey)
        {
           var tableEntity = await GetEntityByPartitionKeyAndRowKey(partitionKey, rowKey);
           if (tableEntity is not null)
           { 
               TableOperation tableOperation = TableOperation.Delete(tableEntity);
               await _cloudTable.ExecuteAsync(tableOperation);
               return true;
           }
            return false;
        }

        public async Task<List<UserDAL>> RunDemo()
        {
            try
            {
                await CreateNewTable();
                var listOfFakes = GetFakeData();
                listOfFakes.ForEach(x => CreateEntity(x).Wait());
                var list = await GetAllUsers();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<UserDAL> GetFakeData()
        {
            var list = new List<UserDAL>();

            list.Add(new UserDAL("1", "Gentle", "Possel", "gentlebuter@gmail.com", "01/01/2021", 1000, 12000, 25, 900, true));
            list.Add(new UserDAL("2", "Pietje", "Puk", "639567@student.inholland.nl", "01/01/2021", 1000, 12000, 25, 900, true));
            list.Add(new UserDAL("3", "John", "Dohn", "", "01/01/2021", 1000, 12000, 25, 900, true));
            list.Add(new UserDAL("4", "Johhny", "Depp", "", "01/01/2021", 1000, 12000, 25, 900, true));

            return list;
        }

        public async Task<List<UserDAL>> GetAllUsers()
        {
            await CreateNewTable();
            List<UserDAL> lstCus = new List<UserDAL>();

            TableQuery<UserDAL> query = new TableQuery<UserDAL>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, "0"));

            foreach (UserDAL customer in await _cloudTable.ExecuteQuerySegmentedAsync(query, null))
            {
                lstCus.Add(customer);
            }
            return lstCus;
        }

    }
}
