using MassTransit;
using Shared.Classes;
using SyncService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncService.Consumers
{
    public class TestReadyConsumer : IConsumer<TestReadyMessage>
    {
        private readonly ISyncronisationService _syncronisationService;

        public TestReadyConsumer(ISyncronisationService syncronisationService)
        {
            _syncronisationService = syncronisationService;
        }

        public async Task Consume(ConsumeContext<TestReadyMessage> context)
        {
            await _syncronisationService.TestReady(context.Message);
        }
    }
}
