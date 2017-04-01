using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.NotificationHubs;


namespace VikasIoTController
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://vikasgomia.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=BwRmgBo6nEAxm2xtdt6id9FmDACgFYyjtmYoxUEWJNM=",
                                                                         "VikasNotificationHub");
        }
    }

}