using RabbitMQSync.Classes;
using System;

namespace RabbitMQSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseS = new SynchronisationBase();

            SynchronisationHandler.Handle(baseS, () =>
                {
                    Console.WriteLine("Sync'd");
                    baseS.WaitForSync();
                    Console.WriteLine("Hello World");


                });

            Console.ReadKey();
        }
    }
}
