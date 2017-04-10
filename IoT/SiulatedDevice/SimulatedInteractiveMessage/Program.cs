using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;


namespace SimulatedInteractiveMessage
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "VikasIoTHub.azure-devices.net";
        static string deviceKey = "/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";
        static string RPiPrimaryKey = "wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy/bIk=";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            //deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myVirtualDevice", deviceKey));
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("IoTExperiment", RPiPrimaryKey));
            
            SendDeviceToCloudInteractiveMessagesAsync();
            //ReceiveC2DAsync();
            Console.ReadLine();
        }


        private static async void SendDeviceToCloudInteractiveMessagesAsync()
        {
            while (true)
            {
                var interactiveMessageString = "Alert Message";
                var interactiveMessage = new Message(Encoding.ASCII.GetBytes(interactiveMessageString));
                interactiveMessage.Properties["messageType"] = "interactive";

                interactiveMessage.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(interactiveMessage);
                Console.WriteLine("{0} > Sending interactive message: {1}", DateTime.Now, interactiveMessageString);

                Task.Delay(10000).Wait();
            }
        }

        private static async void ReceiveC2DAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while(true)
            {
                Message received = await deviceClient.ReceiveAsync();
                if (received == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(received.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(received);
            }
        }


    }
}