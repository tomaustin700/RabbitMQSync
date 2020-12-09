using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncService.Services
{
    public interface ISyncronisationService
    {
        Task TestReady(TestReadyMessage testName);
        Task RunAction(TestReadyMessage test, IEnumerable<string> machineNames);
    }
}
