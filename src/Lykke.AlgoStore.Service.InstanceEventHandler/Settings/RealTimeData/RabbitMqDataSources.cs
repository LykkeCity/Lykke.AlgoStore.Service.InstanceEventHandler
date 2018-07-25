namespace Lykke.AlgoStore.Service.InstanceEventHandler.Settings.RealTimeData
{
    public class RabbitMqDataSources
    {
        public RabbitMqConfig Candles { get; set; }
        public RabbitMqConfig Trades { get; set; }
        public RabbitMqConfig Functions { get; set; }
    }
}
