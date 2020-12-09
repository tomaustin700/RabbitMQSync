using RabbitMQSync.Classes;
using System;

namespace RabbitMQSync
{
    class Program
    {
        static void Main(string[] args)
        {
            SynchronisationHandler.Handle(new SynchronisationBase(), () =>
                {

                });
        }
    }
}
