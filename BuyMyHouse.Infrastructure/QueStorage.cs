using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace BuyMyHouse.Infrastructure
{
    public class QueStorage
    {
        public CloudQueueClient _queueClient { get; set; }
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public QueStorage()
        {

        }

        public async Task CreateQueueMessage(string queName, string message)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                _queueClient = cloudStorageAccount.CreateCloudQueueClient();
                CloudQueue queue = _queueClient.GetQueueReference(queName);
                await queue.CreateIfNotExistsAsync();

                CloudQueueMessage item = new CloudQueueMessage(message);
                await queue.AddMessageAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
            }
        }


    }
}
