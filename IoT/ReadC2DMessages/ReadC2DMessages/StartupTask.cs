using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using Windows.System;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;


// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ReadC2DMessages
{
    public sealed class StartupTask : IBackgroundTask
    {
            
        static string virtualDeviceID = "myVirtualDevice";
        static string virtualPrimaryKey = "/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";
        static string virtualConnectionstring = "HostName=VikasIoTHub.azure-devices.net;DeviceId=myVirtualDevice;SharedAccessKey=/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";

        static string RPiDeviceID = "IoTExperiment";
        static string RPiPrimaryKey = "wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy";
        static string RPiConnectionstring = "HostName=VikasIoTHub.azure-devices.net;DeviceId=IoTExperiment;SharedAccessKey=wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy/bIk=";

        static DeviceClient deviceClient;
        static string iotHubUri = "VikasIoTHub.azure-devices.net";
        static string deviceKey = virtualPrimaryKey; //" / 6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";
        BackgroundTaskDeferral Deferral;
        private AppServiceConnection _AppService;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            //execution is triggered by another application requesting this AppService
            //assigns an Event handler to fire when a message is received from the client

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _AppService = triggerDetails.AppServiceConnection;
            _AppService.RequestReceived += Connection_RequestReceived;
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(virtualDeviceID, deviceKey), TransportType.Amqp);

            //Send Device to Cloud Message
            //SendDeviceToCloudMessagesAsync();

            //Receive Cloud to Device Message
            await ReceiveC2dAsync();
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            //Deferral.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //a few reasons that you may be interested in.
            switch (reason)
            {
                case BackgroundTaskCancellationReason.Abort:
                    //app unregistered background task (amoung other reasons).
                    break;
                case BackgroundTaskCancellationReason.Terminating:
                    //system shutdown
                    break;
                case BackgroundTaskCancellationReason.ConditionLoss:
                    break;
                case BackgroundTaskCancellationReason.SystemPolicy:
                    break;
            }
            Deferral.Complete();
        }

        private async Task ReceiveC2dAsync()
        {
            //Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                //Notify client of state
                await NotifyClient();
                if (receivedMessage == null) continue;

                //Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                await deviceClient.CompleteAsync(receivedMessage);

                var res = await Launcher.QueryUriSupportAsync(new Uri("http://www.microsoft.com"), LaunchQuerySupportType.Uri);

            }

        }

        private async Task NotifyClient()
        {
            var messages = new ValueSet();
            messages.Add("Notification", "Signalled");

            //send message to client
            var response = await _AppService.SendMessageAsync(messages);

            if(response.Status == AppServiceResponseStatus.Success)
            {
                var result = response.Message["Response"];
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs e)
        {
            var requestDeferral = e.GetDeferral();
            var returnMessage = new ValueSet();

            try {
                //obtain an react to the message passed in by client
                var message = e.Request.Message["Request"] as string;
                switch (message)
                {
                    case "Turn LED On":
                        //_ledPin.Write(GpioPinValue.High);
                        break;
                    case "Turn LED Off":
                        //_ledPin.Write(GpioPinValue.Low);
                        break;
                }
                returnMessage.Add("Response", "OK");
            }
            catch(Exception ex)
            {
                returnMessage.Add("Response", "Failed: " + ex.Message);
            }
            await e.Request.SendResponseAsync(returnMessage);

            requestDeferral.Complete();
        }
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (Deferral != null)
            {
                Deferral.Complete();
            }
        }
    }
}
