using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSync.Classes
{
    public class TestReadyMessage
    {
        public string SubscriptionId { get; set; }
        public int ListenerCount { get; set; } = 2;
        public static string MachineName
        {
            get
            {
                return Environment.MachineName;
            }
        }
    }
}
