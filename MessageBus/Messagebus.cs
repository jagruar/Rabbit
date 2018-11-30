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
        public const string _DirectExchange = "directExchange";
        public const string _AdminExchange = "adminExchange";
        public const string _PortalExchange = "portalExchange";
        public const string _AdminQueue = "adminQueue";
        public const string _PortalQueue = "portalQueue";

        private IConnection _Connection;
        private List<IModel> Channels;

        public RabbitMessagebus()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this._Connection = factory.CreateConnection();
            this.Channels = new List<IModel>();
        }

        public void PublishOne(string queue, string message)
        {
            using (var channel = _Connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: _DirectExchange, type: "direct");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: _DirectExchange,
                             routingKey: queue,
                             basicProperties: null,
                             body: body);
            }
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

        public void SubscribeAsWorker(string queue, EventHandler<BasicDeliverEventArgs> messageHandler)
        {
            var channel = _Connection.CreateModel();

            channel.ExchangeDeclare(exchange: _DirectExchange, type: "direct");

            channel.QueueDeclare(queue, exclusive: false);
            channel.QueueBind(queue: queue,
                              exchange: _DirectExchange,
                              routingKey: queue);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += messageHandler;

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

        public void Subscribe(string exchange, string queue, EventHandler<BasicDeliverEventArgs> messageHandler)
        {
            var channel = _Connection.CreateModel();            

            channel.ExchangeDeclare(exchange: exchange, type: "fanout");

            channel.QueueDeclare(queue);
            channel.QueueBind(queue: queue,
                              exchange: exchange,
                              routingKey: "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += messageHandler;

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
