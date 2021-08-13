namespace Bulkie.BuildingBlocks.EventBus.Abstractions
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System.Threading.Tasks;

    public interface IEventBus
    {
        Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event)
              where TIntegrationEvent : IntegrationEvent;
    }
}
