using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Utils;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class RabbitMqHandler<T> : IHandler<T>, IDisposable
    {
        private readonly RabbitMqPublisher<T> _rabbitPublisher;

        public RabbitMqHandler(RabbitMqSubscriptionSettings rabbitMqSettings, ILogFactory logFactory)
        {
            _rabbitPublisher = new RabbitMqPublisher<T>(logFactory, rabbitMqSettings)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new GenericRabbitModelConverter<T>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(rabbitMqSettings))
                .PublishSynchronously()
                .Start();
        }

        public async Task Handle(T message)
        {
            await _rabbitPublisher.ProduceAsync(message);
        }

        public void Dispose()
        {
            _rabbitPublisher?.Stop();
            _rabbitPublisher?.Dispose();
        }
    }
}
