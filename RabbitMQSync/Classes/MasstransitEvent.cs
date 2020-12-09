using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSync.Classes
{
    public class MasstransitEvent
    {
        public object message { get; set; }
        public string[] messageType { get; set; } = new string[] { "urn:message:TestSynchroniser.Classes:TestReadyMessage" };
    }
}
