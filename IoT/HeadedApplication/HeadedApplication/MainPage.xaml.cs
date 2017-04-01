using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.AppService;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeadedApplication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppServiceConnection _appService = null;
        public MainPage()
        {
            this.InitializeComponent();
            SetupAppService();
        }

        private async void SetupAppService()
        {
            //var listing = await AppServiceCatalog.FindAppServiceProvidersAsync("ReceiveMessageService");
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync("C2DService");
            var packageName = "";
            if (listing.Count == 1)
                packageName = listing[0].PackageFamilyName;

            _appService = new AppServiceConnection();
            _appService.PackageFamilyName = packageName;
            _appService.AppServiceName = "C2DService";
                                          

            var status = await _appService.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                //something went wrong
                txtStatus.Text = "Could not connect to the App Service: " + status.ToString();
            }
            else
            {
                //add handler to receive app service messages (Received messages)
                _appService.RequestReceived += _appService_RequestReceived;
            }
            
        }

        private async void _appService_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var requestDefferal = args.GetDeferral();
                var returnMessage = new ValueSet();
                var uiMessage = "";
                var uiMessageBrush = new SolidColorBrush(Colors.Black);

                try
                {
                    var message = args.Request.Message["Notification"] as string;
                    switch(message)
                    {
                        case "Red":
                            uiMessage = "Red!!! Alert!!!";
                            uiMessageBrush = new SolidColorBrush(Colors.Red);
                            break;
                        case "Green":
                            uiMessage = "Green... Move along...";
                            break;
                    }
                    returnMessage.Add("Response", "OK");
                }
                catch(Exception ex)
                { returnMessage.Add("Response", "Failed: " + ex.Message); }

                await args.Request.SendResponseAsync(returnMessage);

                requestDefferal.Complete();

                txtStatus.Text = uiMessage;
                txtStatus.Foreground = uiMessageBrush;
            }); 
        }


        private async void btnCommand_Click(object sender, RoutedEventArgs args)
        {
            Button btn = sender as Button;
            string command = "";
            var message = new ValueSet();
            var StatusColor = new SolidColorBrush(Colors.Black);
            switch (btn.Name)
            {
                case "btnLEDOn":
                    command = "Turn LED On";
                    break;
                case "btnLEDOff":
                    command = "Turn LED Off";
                    break;
            }

            message.Add("Request", command);

            //Use the app service connection to send the LED command
            var response = await _appService.SendMessageAsync(message);
            var result = "";

            //analyze response from background task
            if (response.Status == AppServiceResponseStatus.Success)
                result = response.Message["Response"] as string;
            else
            {
                result = "Something went wrong: " + response.Status;
                StatusColor = new SolidColorBrush(Colors.Red);
            }

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                txtStatus.Text = result;
                txtStatus.Foreground = StatusColor;
            });
        }


    }
}
