using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Classes
{
    public class TestReadyMessage
    {
        public string SubscriptionId { get; set; }
        public int ListenerCount { get; set; } = 2;
        public string MachineName
        {
            get
            {
                return Environment.MachineName;
            }
        }
    }
}
