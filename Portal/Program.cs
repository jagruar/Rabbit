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
            using (var mb = new RabbitMessagebus())
            {
                mb.Subscribe(RabbitMessagebus._PortalExchange, RabbitMessagebus._PortalQueue + id, ClearCache);
                mb.SubscribeAsWorker(RabbitMessagebus._PortalQueue, AlterDatabase);
                while (true)
                {
                    Console.WriteLine("Send to admin (a) or portal(p)?");
                    var loc = Console.ReadLine();
                    Console.WriteLine("Send to one(1) or many(2)?");
                    var type = Console.ReadLine();
                    if (type == "1")
                    {
                        var queue = loc == "a" ? RabbitMessagebus._AdminQueue : RabbitMessagebus._PortalQueue;
                        mb.PublishOne(queue, "");
                    }
                    else
                    {
                        var exchange = loc == "a" ? RabbitMessagebus._AdminExchange : RabbitMessagebus._PortalExchange;
                        mb.PublishAll("", exchange);
                    }
                    Console.ReadLine();
                }
            }            
        }

        public static void ClearCache(object sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine("I am clearing my cache");
        }

        public static void AlterDatabase(object sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine("Altering database...");
            int i = 10;
            while (i > 0)
            {
                Console.Write(i + ", ");
                Thread.Sleep(1000);
                i--;
            }
            Console.WriteLine("Database altered");
        }
    }
}
