using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Text;

namespace CallMethodOnDevice.Controllers
{
    public class MyTestController : Controller
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=kCsQhFDm0DAvJNkt5N2/k98849YibznqTuruxkicJ10=";

        public async Task<ActionResult> Launch()
        {
            await InvokeNow();
            return View();
        }

        [System.Web.Http.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
  
        public async Task<ActionResult> submitFormLaunch()
        {
            // do something with the model, such as inserting it into the database.
            // model.Firstname will contain the value of the firstname textbox
            // model.Surname will contain the value of the surnaem textbox
            await InvokeNow();
            return View("Launch");
            // return RedirectToAction("Success");
        }

        [System.Web.Http.HttpPost]
        [System.Web.Mvc.AllowAnonymous]

        public async Task<ActionResult> submitFormSend()
        {
            // do something with the model, such as inserting it into the database.
            // model.Firstname will contain the value of the firstname textbox
            // model.Surname will contain the value of the surnaem textbox
            await InvokeNow();
            return View("Launch");
            // return RedirectToAction("Success");
        }
        public async Task InvokeNow()
        {
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            await SendCloudToDeviceMessageAsync();
        }
        private async static Task SendCloudToDeviceMessageAsync()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await serviceClient.SendAsync("IoTExperiment", commandMessage);
        }

    }

}
