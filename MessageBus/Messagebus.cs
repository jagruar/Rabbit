using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Models;


namespace MessageBus
{
    public class RabbitMessagebus : IDisposable
    {
        public const string _Admin = "admin";
        public const string _Portal = "portal";
        public const string __WebFramewrokExchange = "webframework";

        private IConnection _Connection;
        private List<IModel> Channels;

        public RabbitMessagebus()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this._Connection = factory.CreateConnection();
            this.Channels = new List<IModel>();
        }

        public void PublishAll(string message, string exchange)
        {
            using (var channel = _Connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange, type: "fanout");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchange,
                             routingKey: string.Empty,
                             basicProperties: null,
                             body: body);
            }
        }             

        public void Subscribe(string exchange, string queue, Func<object, BasicDeliverEventArgs, > func)
        {
            var channel = _Connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchange, type: "fanout");

            channel.QueueDeclare(queue);
            channel.QueueBind(queue: queue,
                              exchange: exchange,
                              routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += func;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] {0}", message);
            };

            channel.BasicConsume(queue: queue,
                             autoAck: true,
                             consumer: consumer);
            this.Channels.Add(channel);
        }

        public void Dispose()
        {
            
        }
    }
}
