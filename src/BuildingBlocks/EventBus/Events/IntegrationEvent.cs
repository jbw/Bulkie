
namespace Bulkie.BuildingBlocks.EventBus.Events
{
    using System;

    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public DateTime CreationDate { get; private set; }
    }
}
