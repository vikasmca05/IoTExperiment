using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VikasIoTController
{
    public class D2CEvent
    {
        //  SELECT TOP(1000) [D2CEventId]
        //,[EnqueuedTimeUtc]
        //,[Offset]
        //,[PartitionKey]
        //,[SequenceNumber]
        //,[deviceid]
        //  FROM[dbo].[D2CEvent]

        public string D2CEventId { get; set; }
        public string Offset { get; set; }
        public DateTime EnqueuedTimeUtc { get; set; }
        public string PartitionKey { get; set; }
        public int SequenceNumber { get; set; }
        public string deviceid { get; set; }

    }


    public class D2CEventRepository
    {

        public IEnumerable<D2CEvent> GetData()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [D2CEventId]
                      ,[EnqueuedTimeUtc]
                      ,[Offset]
                      ,[PartitionKey]
                      ,[SequenceNumber]
                      ,[deviceid]
                  FROM [dbo].[D2CEvent]", connection))
                    {
                        // Make sure the command object does not already have
                        // a notification object associated with it.
                        command.Notification = null;

                        SqlDependency dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        using (var reader = command.ExecuteReader())
                            return reader.Cast<IDataRecord>()
                            .Select(x => new D2CEvent()
                            {
                                D2CEventId = x.GetString(0),
                                EnqueuedTimeUtc = x.GetDateTime(1),
                                Offset = x.GetString(2),
                                PartitionKey = x.GetString(3),
                                SequenceNumber = x.GetInt32(4),
                                deviceid = x.GetString(5)
                                
                            }).ToList();



                }
            }
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            D2CEventHub d2c = new D2CEventHub();
            d2c.Show();
        }


    }
}