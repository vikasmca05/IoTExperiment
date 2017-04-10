using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VikasIoTController.Models
{
    public class SendC2DViewModel
    {
        public string DeviceID { get; set; }
        public MessageCommand RemoteCommandId { get; set; }
        public string RemoteCommandName { get; set; }
    }
        public enum MessageCommand
        {
            Command1,
            Command2,
            Command3

        }

    

}