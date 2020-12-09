using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSync.Classes
{
    public static class SynchronisationHandler
    {
        public static SynchronisationOptions SynchronisationOptions { get; private set; }

        public static void Handle(SynchronisationOptions synchronisationOptions, SynchronisationBase syncBase, Action method)
        {
            try
            {

                SynchronisationOptions = synchronisationOptions;

                if (string.IsNullOrEmpty(SynchronisationOptions.TerminationId))
                    SynchronisationOptions.TerminationId = "TestName"; //syncBase.TestName;

                using (var syncConnection = SynchronisationOptions.ConnectionFactory.CreateConnection())
                using (var syncModel = syncConnection.CreateModel())
                {
                    syncBase.WaitForAbort(syncModel);
                    syncBase.WaitForSync();
                    method.Invoke();
                }
            }
            catch (Exception ex)
            {
                syncBase.Abort();
                throw ex;
            }
        }

        public static void Handle(SynchronisationBase syncBase, Action method)
        {
            Handle(new SynchronisationOptions(), syncBase, method);
        }
    }
}
