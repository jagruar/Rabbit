using System;
using MessageBus;

namespace Admin
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = new Random().Next(100);
            var queue = "admin" + id;
            Console.WriteLine("Subscribing...");
            using(var mb = new RabbitMessagebus())
            {
                mb.Subscribe(RabbitMessagebus._Admin, queue);
            }
            Console.WriteLine("Subscribed to Admin Exchange");

            while (true)
            {
                Console.WriteLine("Where should your message be sent?");
                var exchange = Console.ReadLine();
                Console.WriteLine("Enter message:");
                var message = Console.ReadLine();
                using (var mb = new RabbitMessagebus())
                {
                    mb.PublishAll(message, exchange);
                }
                Console.WriteLine($"Sent {message} to {exchange}");
            }            
        }
    }
}
