using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IoT_SimulationDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "your project name";
        static string deviceKey = "your device key";
        private static async void SendDeviceToCloudMessagesAsync()
        {
            double minLenght = 0.01;
            int messageId = 1;
            Random rand = new Random();

            while (true)
            {
                double currentLenght = minLenght + rand.NextDouble() * 15;

                var lenghtDataPoint = new
                {
                    messageId = messageId++,
                    deviceId = "yout device identity",
                    lenght = currentLenght
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("IsFar", (currentLenght > 1) ? "true" : "false");

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("DemoIOTSimulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("your device identity", deviceKey), TransportType.Mqtt);

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
