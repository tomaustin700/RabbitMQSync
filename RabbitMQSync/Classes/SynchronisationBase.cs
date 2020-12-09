using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace RabbitMQSync.Classes
{
    public class SynchronisationBase
    {
        public void WaitForAbort(IModel model)
        {

            var machineGuid = Environment.MachineName + SynchronisationHandler.SynchronisationOptions.TerminationId;

            model.ExchangeDeclare(SynchronisationHandler.SynchronisationOptions.TerminationId, ExchangeType.Fanout, durable: true, autoDelete: true);

            model.QueueDeclare(queue: machineGuid,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: true,
                                 arguments: null);

            model.QueueBind(machineGuid, SynchronisationHandler.SynchronisationOptions.TerminationId, SynchronisationHandler.SynchronisationOptions.TerminationId);

            var abortConsumer = new EventingBasicConsumer(model);

            abortConsumer.Received += (model, ea) =>
            {
                //TestHandler.RemoteTerminationRequested = true;
                //TestHandler.CleanUp(this);

                //Abort
            };

            model.BasicConsume(queue: Environment.MachineName + SynchronisationHandler.SynchronisationOptions.TerminationId,
                             autoAck: true, consumer: abortConsumer);
        }
        public void Abort()
        {
            using (var connection = SynchronisationHandler.SynchronisationOptions.ConnectionFactory.CreateConnection())
            using (var channelSend = connection.CreateModel())
            {
                channelSend.BasicPublish(exchange: SynchronisationHandler.SynchronisationOptions.TerminationId,
                                    routingKey: SynchronisationHandler.SynchronisationOptions.TerminationId,
                                    basicProperties: null,
                                    body: Encoding.UTF8.GetBytes("ABORT"));

            }
        }


        public void WaitForSync([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            if (SynchronisationHandler.SynchronisationOptions == null)
                throw new InvalidOperationException(@"{} must be set when syncing with other instances");
            else if (SynchronisationHandler.SynchronisationOptions.ListenerCount == 1)
                throw new InvalidOperationException("ListenerCount should never be 1, who are you syncing with?");


            bool ready = false;

            var id = !string.IsNullOrEmpty(SynchronisationHandler.SynchronisationOptions.SyncId) ? SynchronisationHandler.SynchronisationOptions.SyncId : lineNumber + caller + "TestName"; //TestName;

            var machineGuid = Environment.MachineName + id;

            using (var connection = SynchronisationHandler.SynchronisationOptions.ConnectionFactory.CreateConnection())
            using (var channelSend = connection.CreateModel())
            using (var channelRec = connection.CreateModel())
            {
                channelSend.QueueDeclare(queue: "test_ready",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channelRec.ExchangeDeclare(id, ExchangeType.Fanout, durable: true, autoDelete: true);

                channelRec.QueueDeclare(queue: machineGuid,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: true,
                                     arguments: null);

                channelRec.QueueBind(machineGuid, id, id);



                var massEvent = new MasstransitEvent();
                massEvent.message = new TestReadyMessage() { SubscriptionId = id, ListenerCount = SynchronisationHandler.SynchronisationOptions.ListenerCount };

                var body = Encoding.Default.GetBytes(JsonConvert.SerializeObject(massEvent));

                IBasicProperties props = channelSend.CreateBasicProperties();
                props.ContentType = "application/vnd.masstransit+json";

                var consumer = new EventingBasicConsumer(channelRec);

                consumer.Received += (model, ea) =>
                {
                    ready = true;
                };

                channelRec.BasicConsume(queue: machineGuid,
                                 autoAck: true,
                                 consumer: consumer);

                channelSend.BasicPublish(exchange: "",
routingKey: "test_ready",
                                     basicProperties: props,
                                     body: body);

                SHSpinWait.SpinUntil(() => ready, timeout: TimeSpan.FromMinutes(5));

                //if (!ready)
                //    LoggingService.Info("Timed out waiting for synchronisation with another session");

            }
        }
    }
}
