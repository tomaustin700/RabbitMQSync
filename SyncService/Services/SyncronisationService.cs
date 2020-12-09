using MassTransit;
using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncService.Services
{
    public class SyncronisationService : ISyncronisationService
    {
        private readonly IBusControl _bus;
        public SyncronisationService(IBusControl bus)
        {
            _bus = bus;
        }
        private List<TestReadyMessage> _listeners = new List<TestReadyMessage>();

        public async Task TestReady(TestReadyMessage test)
        {
            // using ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Redis.RedisConnectionString);
            //IDatabase db = redis.GetDatabase();

            //var listeners = new List<TestReadyMessage>();

            //if (db.KeyExists("SyncService_TestReady_Listeners"))
            //{
            //    var listenersJson = db.StringGet("SyncService_TestReady_Listeners");

            //    listeners = JsonConvert.DeserializeObject<List<TestReadyMessage>>(listenersJson);
            //}

            //if (!listeners.Any(x => x.SubscriptionId == test.SubscriptionId)) //&& x.MachineName == test.MachineName))
            _listeners.Add(test);

            if (_listeners.Count(x => x.SubscriptionId == test.SubscriptionId) >= test.ListenerCount)
            {
                await RunAction(test, _listeners.Where(x => x.SubscriptionId == test.SubscriptionId).Select(a => a.MachineName));
                _listeners.RemoveAll(x => x.SubscriptionId == test.SubscriptionId);
            }

            //db.StringSet("SyncService_TestReady_Listeners", JsonConvert.SerializeObject(listeners));


        }

        public async Task RunAction(TestReadyMessage test, IEnumerable<string> machineNames)
        {
            foreach (var machineName in machineNames)
            {
                Uri uri = new Uri($"rabbitmq://127.0.01/{test.SubscriptionId}?autodelete=true");
                var endPoint = await _bus.GetSendEndpoint(uri);

                await endPoint.Send(test);
            }
        }
    }
}
