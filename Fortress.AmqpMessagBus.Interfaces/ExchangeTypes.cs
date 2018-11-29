namespace Fortress.MessagBus.Interfaces
{
    /// <summary>
    /// The type of AMQP exchanges.
    /// </summary>
    public enum ExchangeTypes
    {
        Direct = 1,
        Fanout = 2,
        Topic = 3,
        Headers = 4
    }
}
