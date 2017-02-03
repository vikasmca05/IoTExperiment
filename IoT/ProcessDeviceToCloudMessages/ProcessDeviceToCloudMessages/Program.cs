using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ProcessDeviceToCloudMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHubConnectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=UaHW2M62P5MxHsJXtd5fK+ZD8H6LZ1ww/55QnoleMrI=";
            string iotHubD2cEndpoint = "messages/events";
            StoreEventProcessor.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=vikasblobstorage;AccountKey=nAzhCANq1mIMCMi1XCp7BQByA8o76n2XrZtXbzUVCdMiNRJBTlN6F9XMZ9OQ6v0zjzjzmNLN8D3XSAhHw6qVVQ==";
            StoreEventProcessor.ServiceBusConnectionString = "Endpoint=sb://vikassb.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=dYtVS+q8QAOHoVKRbnAHk114yj9ZAjJ3dSOKMLg8BkU=;EntityPath=vikasqueue";//"Endpoint=sb://vikassb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Ypr/bf4mLnuocqehs9g6nfo60LWobDTY6MbDvJvkwu8=";

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, iotHubD2cEndpoint, EventHubConsumerGroup.DefaultGroupName, iotHubConnectionString, StoreEventProcessor.StorageConnectionString, "messages-events");
            Console.WriteLine("Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<StoreEventProcessor>().Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
