using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Text;
using System.Threading;

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
        private static List<ManualResetEvent> _waitHandles = null;
        int defaultMaxworkerThreads = 0;
        int defaultmaxIOThreads = 0;
        int waitHandleIndex = 0;
        int maxThreadsAtOnce = 35;

        static string virtualDeviceID = "myVirtualDevice";
        static string virtualPrimaryKey = "/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";
        static string virtualConnectionstring = "HostName=VikasIoTHub.azure-devices.net;DeviceId=myVirtualDevice;SharedAccessKey=/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";

        static string RPiDeviceID = "IoTExperiment";
        static string RPiPrimaryKey = "wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy";
        static string RPiConnectionstring = "HostName=VikasIoTHub.azure-devices.net;DeviceId=IoTExperiment;SharedAccessKey=wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy/bIk=";

        static ServiceClient serviceClient;
        static string connectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=UaHW2M62P5MxHsJXtd5fK+ZD8H6LZ1ww/55QnoleMrI=";

        public void StartNow(string id)
        {
            System.Diagnostics.Trace.TraceInformation("Send Cloud to Device Message");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            System.Diagnostics.Trace.TraceInformation("Press any key to send C2D message");

            var numThreads = 1;
            var toProcess = numThreads;

            try { 
                var resetEvent = new ManualResetEvent(false);

                for (var i = 0; i < numThreads; i++)
                {
                    ThreadPool.QueueUserWorkItem(
                        new WaitCallback(delegate (object state) {
                            var strMessage = "1";
                            SendC2DMessage(strMessage);
                            if (Interlocked.Decrement(ref toProcess) == 0) resetEvent.Set();
                        }), null);
                    
                }

                resetEvent.WaitOne();
            }
            catch (Exception ex)
            {
             
            }


        }

        public void PushNotifications()
        {
        }

        public void ReadStorageForNewEvents()
        {
            //Read storage DB
            D2CEventRepository objRepo = new D2CEventRepository();
            IEnumerable<D2CEvent> data = objRepo.GetData();
        }
          

        public void ChangeEvent()
        {
            System.Diagnostics.Trace.TraceInformation("Change Event Method called");
            System.Diagnostics.Trace.TraceInformation("Try to Read from Database for the new Events");
            ReadStorageForNewEvents();
            System.Diagnostics.Trace.TraceInformation("Try to Send Notification to User");
            
        }

        private static async void ReceiveC2DAsync()
        {
            System.Diagnostics.Trace.TraceInformation("Receiving cloud to device messages from service");
            while (true)
            {
                
            }
        }
        public static void SendC2DMessage(string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            serviceClient.SendAsync(RPiDeviceID, commandMessage).Wait();
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();
            System.Diagnostics.Trace.TraceInformation("Receiving c2d feedback from service");
            
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;
                System.Diagnostics.Trace.TraceInformation("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}