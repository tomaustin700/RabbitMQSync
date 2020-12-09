using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSync.Classes
{
    public class SynchronisationOptions
    {
        public int ListenerCount { get; set; } = 2;
        public string TerminationId { get; set; }
        public IConnectionFactory ConnectionFactory { get; private set; } = new ConnectionFactory() { HostName = "127.0.01" };
        public string SyncId { get; set; }

        public SynchronisationOptions()
        {

        }

        public SynchronisationOptions(IConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }
    }
}
