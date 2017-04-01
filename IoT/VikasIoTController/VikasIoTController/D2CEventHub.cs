using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace VikasIoTController
{
    public class D2CEventHub : Hub
    {
        public void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<D2CEventHub>();
            context.Clients.All.displayStatus();
        }
    }
}