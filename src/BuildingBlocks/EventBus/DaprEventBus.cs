namespace Bulkie.BuildingBlocks.EventBus
{
    using Bulkie.BuildingBlocks.EventBus.Abstractions;
    using Bulkie.BuildingBlocks.EventBus.Events;
    using Dapr.Client;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class DaprEventBus : IEventBus
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        private readonly DaprClient _dapr;
        private readonly ILogger<DaprEventBus> _logger;

        public DaprEventBus(DaprClient dapr, ILogger<DaprEventBus> logger)
        {
            _dapr = dapr;
            _logger = logger;
        }

        public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event) where TIntegrationEvent : IntegrationEvent
        {
            var topicName = @event.GetType().Name;

            _logger.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", @event, DAPR_PUBSUB_NAME, topicName);

            await _dapr.PublishEventAsync(DAPR_PUBSUB_NAME, topicName, (dynamic)@event);
        }
    }

}
