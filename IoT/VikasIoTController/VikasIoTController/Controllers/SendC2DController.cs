using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VikasIoTController.Models;

namespace VikasIoTController.Controllers
{
    public class SendC2DController : Controller
    {
        // GET: SendC2D
        [HttpGet]
        public ViewResult SendMessage()
        {
            SendMessageNow();
          //var messageModel = new SendMessageMode();
            return View();
        }

        [HttpGet]
        public ViewResult SendPushNotifications()
        {
            PushNotifications();
            //var messageModel = new SendMessageMode();
            return View();
        }

        [HttpPost]
        public ActionResult SendMessageCheck()
        {
            SendMessageNow();
            return RedirectToAction("Success");
            //return View("View");
        }
        public void SendMessageNow()
        {
            SendC2DHandler control = new SendC2DHandler();
            control.StartNow();
        }
        public void PushNotifications()
        {
            SendC2DHandler control = new SendC2DHandler();
            control.PushNotifications();
        }

        [HttpGet]
        public ActionResult ChangeEventPosted()
        {
            SendC2DHandler control = new SendC2DHandler();
            control.ChangeEvent();
            return RedirectToAction("Success");
            //return View("View");
        }

    }
}