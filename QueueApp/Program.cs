using System;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Queues;

namespace QueueApp
{
    class Program
    {
        private static string ConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=fvoice;AccountKey=HfiTpyl8Iri7YJB/CArpXGssbqO7mB4cp4Hj2MkWMavxg3QH/c/APuiTtiX8f4HXYDUVDVRMNZGK5ozkl9+uNg==;EndpointSuffix=core.windows.net";

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                string value = String.Join(" ", args);
                await SendArticleAsync(value);
                Console.WriteLine($"Sent: {value}");
            }
            else
            {
                string value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
            }
        }

        static async Task SendArticleAsync(string newsMessage)
        {
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            //CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            //CloudQueue queue = queueClient.GetQueueReference("newsqueue");
            //bool createdQueue = await queue.CreateIfNotExistsAsync();
            //if (createdQueue)
            //{
            //    Console.WriteLine("The queue of news articles was created.");
            //}

            //CloudQueueMessage articleMessage = new CloudQueueMessage(newsMessage);
            //await queue.AddMessageAsync(articleMessage);


            // Get a reference to a queue and then create it
            QueueClient queue = new QueueClient(ConnectionString, "queue2");
            await queue.CreateIfNotExistsAsync();

            // Send a message to our queue
            await queue.SendMessageAsync(newsMessage);
        }

        //static CloudQueue GetQueue()
        //{
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

        //    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
        //    return queueClient.GetQueueReference("newsqueue");
        //}

        static async Task<string> ReceiveArticleAsync()
        {
            QueueClient queue = new QueueClient(ConnectionString, "queue2");
            bool exists = await queue.ExistsAsync();
            if (exists)
            {
                var retrievedArticle = await queue.ReceiveMessageAsync();
                if (retrievedArticle.Value != null)
                {
                    string newsMessage = retrievedArticle.Value.MessageText;
                    await queue.DeleteMessageAsync(retrievedArticle.Value.MessageId, retrievedArticle.Value.PopReceipt);
                    return newsMessage;
                }
            }

            return "<queue empty or not created>";
        }
    }
}