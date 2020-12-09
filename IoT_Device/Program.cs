using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT_Device
{
    class Program
    {
        static RegistryManager registryManager;
        // Add connection string like "HostName=your project host name;SharedAccessKeyName=iothubowner;SharedAccessKey=your shared key"
        static string connectionString = ConfigurationManager.ConnectionStrings["IoTConnection"].ConnectionString;
        private static async Task AddDeviceAsync()
        {
            Device device;
            string deviceID = "yout device identity";

            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceID));
                Console.WriteLine($"Device: \"{deviceID}\" created.");
            }
            catch(DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceID);
                Console.WriteLine($"Device: \"{deviceID}\" connected.");
            }



            Console.WriteLine($"Device: \"{deviceID}\"\t" +
                $"Device key: \"{device.Authentication.SymmetricKey.PrimaryKey}\"");
            Console.WriteLine(device.Scope);
            Console.WriteLine(device.CloudToDeviceMessageCount);
            Console.WriteLine(device.ConnectionStateUpdatedTime);
            Console.WriteLine(device.ETag);
            Console.WriteLine(device.ConnectionState.ToString());
            Console.WriteLine(device.Capabilities.IotEdge);
        }

        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }
    }
}
