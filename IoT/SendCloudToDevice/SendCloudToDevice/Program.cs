using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SendCloudToDevice
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=UaHW2M62P5MxHsJXtd5fK+ZD8H6LZ1ww/55QnoleMrI=";
        static void Main(string[] args)
        {
            System.Diagnostics.Trace.TraceInformation("Send Cloud to Device Message");
      
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            System.Diagnostics.Trace.TraceInformation("Press any key to send C2D message");
            SendC2DMessage().Wait();
        }

        private async static Task SendC2DMessage()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await serviceClient.SendAsync("myVirtualDevice", commandMessage);
            //await serviceClient.SendAsync("IoTExperiment", commandMessage);
        }
    }
}
