using System;
using System.Collections.Generic;
using System.Text;

namespace Fortress.MessagBus.Interfaces
{
    public interface IAmqpMessageBus : IDisposable
    {
        void Subscribe(string exchange, string queue, EventHandler<> func, ExchangeTypes exchangeType = ExchangeTypes.Direct);
        void PublishToAll(string message, string exchangeName, ExchangeTypes exchangeType = ExchangeTypes.Direct);
        void PublishOnce(string message, string exchangeName, ExchangeTypes exchangeType = ExchangeTypes.Direct);
    }
}
