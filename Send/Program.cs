using System;
using RabbitMQ.Client;
using System.Text;

namespace Send
{
    class Program
    {
        static void Main()
        {



            var exit = false;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                while(!exit)
                {
                    var input = Console.ReadLine();
                    if (input == "x")
                    {
                        exit = true;
                        break;
                    }
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: input,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null
                            );

                        string message = "Hello World";

                        IBasicProperties properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                     routingKey: input,
                                     basicProperties: null,
                                     body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                    Console.WriteLine(" Press x to exit.");
                }                
            }
        }
    }
}
