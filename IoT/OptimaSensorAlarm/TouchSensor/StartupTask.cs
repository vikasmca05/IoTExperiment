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
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Newtonsoft.Json;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace TouchSensor
{
    public sealed class StartupTask : IBackgroundTask
    {

        static string virtualDeviceID = "myVirtualDevice";
        static string virtualPrimaryKey = "/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";
        static string virtualConnectionstring = "HostName=VikasIoTHub.azure-devices.net;DeviceId=myVirtualDevice;SharedAccessKey=/6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";

        static string RPiDeviceID = "IoTExperiment";
        static string RPiPrimaryKey = "wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy/bIk=";
        static string RPiConnectionstring = "HostName=VikasIoTHub.azure-devices.net;DeviceId=IoTExperiment;SharedAccessKey=wLs8zB9W8g8I8BQwPC2S4hGl0mKg1V4WJzgV7hy/bIk=";

        static DeviceClient deviceClient;
        static string iotHubUri = "VikasIoTHub.azure-devices.net";
        static string deviceKey = RPiPrimaryKey; //" / 6t9feUPbOyQj/NLnwbteaDYbULQ33r5X89tW4iTn4M=";
        BackgroundTaskDeferral Deferral;
        private AppServiceConnection _AppService;

        //LED Controls
        private int LEDStatus = 0;
        private const int REDLED_PIN = 5;
        private const int BLUELED_PIN = 6;
        private const int GREENLED_PIN = 13;
        private GpioPin redpin;
        private GpioPin bluepin;
        private GpioPin greenpin;
        private DispatcherTimer timer;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            //execution is triggered by another application requesting this AppService
            //assigns an Event handler to fire when a message is received from the client

            //var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            //_AppService = triggerDetails.AppServiceConnection;
            //_AppService.RequestReceived += Connection_RequestReceived;


            //Send Device to Cloud Message
            SendDeviceToCloudMessagesAsync();
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(RPiDeviceID, deviceKey), TransportType.Amqp);
            //Send Device to Cloud Interactive Message
            SendDeviceToCloudInteractiveMessagesAsync();

            //Change LED State 

            InitGPIO();
            
            //Receive Cloud to Device Message
            ReceiveC2dAsync();
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            //Deferral.Complete();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
              var telemetryDataPoint = new
                {
                    deviceId = RPiDeviceID
    
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                
            
        }
        private static async void SendDeviceToCloudInteractiveMessagesAsync()
        {
            var interactiveMessageString = "Alert Message";
            var interactiveMessage = new Message(Encoding.ASCII.GetBytes(interactiveMessageString));
            interactiveMessage.Properties["messageType"] = "interactive";

            interactiveMessage.MessageId = Guid.NewGuid().ToString();

            await deviceClient.SendEventAsync(interactiveMessage);
                                    
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
            // Cleanup
            redpin.Dispose();
            bluepin.Dispose();
            greenpin.Dispose();

            Deferral.Complete();
        }

        private async Task ReceiveC2dAsync()
        {
            //deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(RPiDeviceID, deviceKey), TransportType.Amqp);
            //Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
           
                if (receivedMessage == null) continue;

                //LED Control
                //timer = new DispatcherTimer();
                //timer.Interval = TimeSpan.FromMilliseconds(500);
                //timer.Tick += Timer_Tick;
                //timer.Start();
                FlipLED();
                ////Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                await deviceClient.CompleteAsync(receivedMessage);

                
                ////Notify client of state
                //await NotifyClient();
                //var res = await Launcher.QueryUriSupportAsync(new Uri("http://www.microsoft.com"), LaunchQuerySupportType.Uri);

            }

        }

        private void Timer_Tick(object sender, object e)
        {
            FlipLED();
        }

        

        private void FlipLED()
        {
            if (LEDStatus == 0)
            {
                LEDStatus = 1;
                if (redpin != null && bluepin != null && greenpin != null)
                {
                    //turn on red
                    redpin.Write(GpioPinValue.High);
                    bluepin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.Low);
                }
                
            }
            else if (LEDStatus == 1)
            {
                LEDStatus = 2;
                if (redpin != null && bluepin != null && greenpin != null)
                {
                    //turn on blue
                    redpin.Write(GpioPinValue.Low);
                    bluepin.Write(GpioPinValue.High);
                    greenpin.Write(GpioPinValue.Low);
                }
                
            }

            else
            {
                LEDStatus = 0;
                if (redpin != null && bluepin != null && greenpin != null)
                {
                    //turn on green
                    redpin.Write(GpioPinValue.Low);
                    bluepin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.High);
                }
                
            }
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                redpin = null;
                bluepin = null;
                greenpin = null;
                return;
            }

            redpin = gpio.OpenPin(REDLED_PIN);
            bluepin = gpio.OpenPin(BLUELED_PIN);
            greenpin = gpio.OpenPin(GREENLED_PIN);

            redpin.Write(GpioPinValue.High);
            redpin.SetDriveMode(GpioPinDriveMode.Output);
            bluepin.Write(GpioPinValue.High);
            bluepin.SetDriveMode(GpioPinDriveMode.Output);
            greenpin.Write(GpioPinValue.High);
            greenpin.SetDriveMode(GpioPinDriveMode.Output);

        }

        private async Task NotifyClient()
        {
            var messages = new ValueSet();
            messages.Add("Notification", "Signalled");

            //send message to client
            var response = await _AppService.SendMessageAsync(messages);

            if (response.Status == AppServiceResponseStatus.Success)
            {
                var result = response.Message["Response"];
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs e)
        {
            var requestDeferral = e.GetDeferral();
            var returnMessage = new ValueSet();

            try
            {
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
            catch (Exception ex)
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
