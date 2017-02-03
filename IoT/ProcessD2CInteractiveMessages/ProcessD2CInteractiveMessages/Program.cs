using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace ProcessD2CInteractiveMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Process D2C Interactive messages \n");
            string connectionString = "Endpoint=sb://vikassb.servicebus.windows.net/;SharedAccessKeyName=listen;SharedAccessKey=2Xzp3C/RIW8BeqzpIhGqB3cimnOx6aPo+REqi7yw65M=;EntityPath=vikasqueue";

            QueueClient Client = QueueClient.CreateFromConnectionString(connectionString);

            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            Client.OnMessage((message) =>
            {
                try
                {
                    var bodyStream = message.GetBody<Stream>();
                    bodyStream.Position = 0;
                    var bodyAsString = new StreamReader(bodyStream, Encoding.ASCII).ReadToEnd();

                    Console.WriteLine("Received message: {0} messageId: {1}", bodyAsString, message.MessageId);

                    message.Complete();

                }

                catch (Exception)
                {
                    message.Abandon();
                }
            }, options);


            Console.WriteLine("Receiving interactive messages from SB queue...");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
