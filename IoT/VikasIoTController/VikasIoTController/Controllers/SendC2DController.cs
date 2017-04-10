using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VikasIoTController.Models;

namespace VikasIoTController.Controllers
{
    public class SendC2DController : Controller
    {
        static string connectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=UaHW2M62P5MxHsJXtd5fK+ZD8H6LZ1ww/55QnoleMrI=";
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> Index()
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            IEnumerable<Device> devices = await registryManager.GetDevicesAsync(100);

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Blue", Value = "0" });
            items.Add(new SelectListItem { Text = "Green", Value = "1" });
            items.Add(new SelectListItem { Text = "Red", Value = "2", Selected = true });

            ViewBag.CommandType = items;
            return View(devices);
        }

        public ActionResult SendMessage(string id)
        {
            SendMessageNow(id);
            return RedirectToAction("../Home/Index");
        }

   


        public async Task<ActionResult> SendPushNotifications()
        {
            PushNotifications();
            //var messageModel = new SendMessageMode();
            return View();
        }


        public async Task<ActionResult> SendMessageCheck(SendC2DViewModel model)
        {
           // SendMessageNow(model);
            return RedirectToAction("Success");
            //return View("View");
        }
        public void SendMessageNow(string id)
        {
            SendC2DHandler control = new SendC2DHandler();
            control.StartNow(id);
        }
        public void PushNotifications()
        {
            SendC2DHandler control = new SendC2DHandler();
            control.PushNotifications();
        }

        public async Task<ActionResult> ChangeEventPosted()
        {
            SendC2DHandler control = new SendC2DHandler();
            control.ChangeEvent();
            return RedirectToAction("Success");
            //return View("View");
        }

        public async void GetDeviceInventory()
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(ConfigurationManager.AppSettings[connectionString]);
            var devices = await registryManager.GetDevicesAsync(-1);


        }
    }
}