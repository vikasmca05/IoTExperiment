using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using AppBackend.Models;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace VikasIoTController.Controllers
{
    public class NotificationsController : Controller
    {

        public async Task<ActionResult> SendToastMessage()

        //public async Task<HttpResponseMessage> Post(string pns, [FromBody]string message, string to_tag)
        {
            //var user = HttpContext.Current.User.Identity.Name;
            //string[] userTag = new string[2];
            //userTag[0] = "username:" + to_tag;
            //userTag[1] = "from:" + user;
            string Pushmessage = null;
            string UserTag = null;
            string fromUser = null;

            if (Request.ContentLength != null)
            {
                Pushmessage = Request.QueryString["message"];
                UserTag = Request.QueryString["to_tag"];
                fromUser = Request.QueryString["user"];
            }
            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            switch ("wns")
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + "From " + fromUser + ": " + Pushmessage + "</text></binding></visual></toast>";
                    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast,UserTag);
                    break;
                //case "apns":
                //    // iOS
                //    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
                //    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                //    break;
                //case "gcm":
                //    // Android
                //    var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + message + "\"}}";
                //    outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
                //    break;
            }

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

            //return View();
            return RedirectToAction("Success");
        }
    }
}
