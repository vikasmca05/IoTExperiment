using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Net;

namespace ProcessD2CMessagesWebjob
{
    class Program
    {
        static string connectionString = "HostName=VikasIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=UaHW2M62P5MxHsJXtd5fK+ZD8H6LZ1ww/55QnoleMrI=";
        static string iotHubD2cEndpoint = "messages/events";
        //static string storageconnectionstring = "Server=tcp:antitheft-vikas.database.windows.net,1433;Initial Catalog = AntiTheft; Persist Security Info=False;User ID = vikas@antitheft-vikas; Password=Sonam123!; MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30";
        static string storageconnectionstring = "Server=tcp:antitheft-vikas.database.windows.net,1433;Initial Catalog=AntiTheft;User ID=vikas;Password=Sonam123!;";

        static EventHubClient eventHubClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
                await UpdateMessagingStorage(eventData);
            }
        }

        private static async Task UpdateMessagingStorage(EventData data)
        {
            SqlConnection connection = new SqlConnection(storageconnectionstring);

            try
            {
                string _guid = Guid.NewGuid().ToString();
                string Sensordata = Encoding.UTF8.GetString(data.GetBytes());
                JObject o = JObject.Parse(Sensordata);
                var deviceKey = o["deviceId"];
                


                connection.Open();
                //D2CEventId int Not NULL,
                //EnqueuedTimeUtc DateTime,
                //Offset varchar,
                //PartitionKey varchar,
                //SequenceNumber int,
                //SerializedSizeInBytes int,
                //deviceid varchar,
                //PRIMARY KEY (D2CEventId)

                SqlCommand insertCommand = new SqlCommand("INSERT INTO D2CEvent (D2CEventId, EnqueuedTimeUtc, Offset, SequenceNumber, deviceid) VALUES (@0, @1, @2, @3, @4)", connection);
                insertCommand.Parameters.AddWithValue("@0", _guid);
                insertCommand.Parameters.AddWithValue("@1", data.EnqueuedTimeUtc);
                insertCommand.Parameters.AddWithValue("@2", data.Offset);
                //insertCommand.Parameters.AddWithValue("@3", data.PartitionKey);
                insertCommand.Parameters.AddWithValue("@3", data.SequenceNumber);
                insertCommand.Parameters.AddWithValue("@4", deviceKey.ToString());
                Console.WriteLine(insertCommand.CommandText);
                Console.WriteLine("Number of Rows Affected is: {0}", insertCommand.ExecuteNonQuery());

                //Insert into Sensor Event
                insertCommand = new SqlCommand("INSERT INTO SensorEvent (SensorEventId, D2CEventId, SensorValue) VALUES (@0, @1, @2)", connection);
                string _Sensorguid = Guid.NewGuid().ToString();
                insertCommand.Parameters.AddWithValue("@0", _Sensorguid);
                insertCommand.Parameters.AddWithValue("@1", _guid);
                insertCommand.Parameters.AddWithValue("@2", "Test Value");
                Console.WriteLine(insertCommand.CommandText);
                Console.WriteLine("Number of Rows Affected is: {0}", insertCommand.ExecuteNonQuery());


            }
            catch (SqlException ex)
            {

                Console.WriteLine(ex.ToString());

            }
            finally
            {

                connection.Close();
                Console.WriteLine("Data base Connection Closed.");

            }
            await NotifyChangeEventStatus();

        }

        public static async Task NotifyChangeEventStatus()
        {
            //Notify MVC Web API
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + "vikasiotcontroller.azurewebsites.net/Sendc2d/ChangeEventPosted");
            request.Method = "GET";
            //specify other request properties

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                HttpStatusCode status = response.StatusCode;
                Console.WriteLine(status.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
