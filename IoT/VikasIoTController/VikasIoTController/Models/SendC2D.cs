using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Text;

namespace VikasIoTController.Models
{
    public class SendMessageMode
    {
        public bool Blue { get; set; }
        public bool Green { get; set; }
        public bool Red { get; set; }
    }
    public class SendC2DHandler
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=UaHW2M62P5MxHsJXtd5fK+ZD8H6LZ1ww/55QnoleMrI=";

        public void StartNow()
        {
            System.Diagnostics.Trace.TraceInformation("Send Cloud to Device Message");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            System.Diagnostics.Trace.TraceInformation("Press any key to send C2D message");
            SendC2DMessage().Wait();
        }


        private async static Task SendC2DMessage()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await serviceClient.SendAsync("IoTExperiment", commandMessage);
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}