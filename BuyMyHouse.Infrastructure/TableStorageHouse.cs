using BuyMyHouse.Domain.Concrete;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyMyHouse.Infrastructure
{
    public class TableStorageHouse : ITableStorage<HouseDAL>
    {
        public CloudTable _cloudTable { get; set; }
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public async Task CreateNewTable()
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                string tableName = "Houses";
                _cloudTable = tableClient.GetTableReference(tableName);
                var result = await _cloudTable.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public async Task<List<HouseDAL>> GetHouses(double min, double max)
        {
            CreateNewTable();
            try
            {
                TableQuery<HouseDAL> query = new TableQuery<HouseDAL>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDouble("Mortgage", QueryComparisons.GreaterThanOrEqual, min),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForDouble("Mortgage", QueryComparisons.LessThanOrEqual, max)
                        )
                    );

                var houseDAL = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);

                return houseDAL.Results;

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<HouseDAL> CreateEntity(HouseDAL entity)
        {
            HouseDAL houseDAL = new HouseDAL();
            try
            {
                TableOperation tableOperation = TableOperation.Insert(entity);
                TableResult result = await _cloudTable.ExecuteAsync(tableOperation);
                houseDAL = result.Result as HouseDAL;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }
            return houseDAL;
        }

        public async Task<HouseDAL> GetEntityByPartitionKeyAndRowKey(string partitionKey, string rowKey)
        {
            await RunDemo();
            try
            {
                TableQuery<HouseDAL> query = new TableQuery<HouseDAL>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)
                        )
                    );

                var houseDAL = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);

                return houseDAL.Results.FirstOrDefault();

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<HouseDAL> UpdateEntity(HouseDAL entity)
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

        public async Task<List<HouseDAL>> RunDemo()
        {
            try
            {
                await CreateNewTable();
                var listOfFakes = GetFakeData();
                listOfFakes.ForEach(x => CreateEntity(x).Wait());
                var list = GetAllHouses();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<HouseDAL> GetFakeData()
        {
            var list = new List<HouseDAL>();

            list.Add(new HouseDAL("0", "Apparment", "On the first floor..", 200000, "www.picture", "Street1", "1", "A", "6460AB", "The Netherlands"));
            list.Add(new HouseDAL("1", "House1", "One floor, one bedrooms", 450000, "www.picture-1", "Street2", "2", "", "3450FT", "The Netherlands"));
            list.Add(new HouseDAL("2", "House2", "Two floors, two bedrooms", 400000, "www.picture-2", "Street3", "3", "", "5500KB", "The Netherlands"));
            list.Add(new HouseDAL("3", "House3", "Three floors, three bedrooms", 450000, "www.picture-3", "Street4", "4", "", "6700KL", "The Netherlands"));

            return list;
        }

        public List<HouseDAL> GetAllHouses()
        {
            List<HouseDAL> lstCus = new List<HouseDAL>();

            TableQuery<HouseDAL> query = new TableQuery<HouseDAL>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, "0"));

            foreach (HouseDAL house in _cloudTable.ExecuteQuerySegmentedAsync(query, null).Result)
            {
                lstCus.Add(house);
            }
            return lstCus;
        }
    }
}
