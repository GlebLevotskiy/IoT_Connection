using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System.Configuration;

namespace IoTConnector
{
    class Program
    {
        // Add connection string like "HostName=your project host name;SharedAccessKeyName=iothubowner;SharedAccessKey=your shared key"
        static string connectionString = ConfigurationManager.ConnectionStrings["IoTConnection"].ConnectionString;
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                EventData eventData = await eventHubReceiver.ReceiveAsync();

                if (eventData == null)
                {
                    continue;
                }

                // Show received message data
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            var tasks = new List<Task>();

            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
