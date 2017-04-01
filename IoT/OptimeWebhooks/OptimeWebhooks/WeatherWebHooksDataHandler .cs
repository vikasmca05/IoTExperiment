using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Properties;
using Newtonsoft.Json.Linq;
using System.IO;

namespace OptimeWebhooks
{
    public class WeatherWebHooksDataHandler : WebHookHandler
    {
        public WeatherWebHooksDataHandler()
        {
            this.Receiver = GenericJsonWebHookReceiver.ReceiverName;
        }

        public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
        {
            // Get data from WebHook
            CustomNotifications data = context.GetDataOrDefault<CustomNotifications>();
            //{"q":"weather in Paris","timezone":"2017-03-01T22:30:49-0800","lang":"en","sessionId":"ba01019e-f563-44c3-9978-cab2e9860a65","resetContexts":false}
            // Get data from each notification in this WebHook
            foreach (IDictionary<string, object> notification in data.Notifications)
            {
                // Process data
            }

         
                JObject incoming = context.GetDataOrDefault<JObject>();
            //{"q":"weather in Paris","timezone":"2017-03-01T22:30:49-0800","lang":"en","sessionId":"ba01019e-f563-44c3-9978-cab2e9860a65","resetContexts":false}
            // Get data from each notification in this WebHook
            JObject JO = new JObject();
            
            string sampleJson = "{ \"speech\": \"Barack\",\"displayText\": \"e \",\"data\": \"Sample data\",\"contextOut\": \"\",\"source\": \"DuckDuckGo\"}";

            string sampleResponse = JO.ToString();
            return Task.FromResult(sampleJson);
            //return Task.FromResult(true);
        }
    }
}
