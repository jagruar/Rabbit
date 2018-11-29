using System;
using System.Threading;
using MessageBus;
using RabbitMQ.Client.Events;

namespace Portal
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = new Random().Next(100);
            var queue = "portal" + id;
            Console.WriteLine("Subscribing...");
            using (var mb = new RabbitMessagebus())
            {
                mb.Subscribe(RabbitMessagebus._Portal, queue);
                while (true)
                {
                    Console.WriteLine("Where should your message be sent?");
                    var exchange = Console.ReadLine();
                    Console.WriteLine("Enter message:");
                    var message = Console.ReadLine();
                    mb.PublishAll(message, exchange);
                    Console.WriteLine($"Sent {message} to {exchange}");
                    Console.ReadLine();
                }
            }            
        }

        public void ClearCache(object sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine("I am clearing my cache");
        }

        public void AlterDatabase(object sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine("Altering database...");
            Thread.Sleep(2000);
            Console.WriteLine("Database altered");
        }
    }
}
