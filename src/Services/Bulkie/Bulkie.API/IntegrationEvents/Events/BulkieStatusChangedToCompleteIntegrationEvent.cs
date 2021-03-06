namespace Bulkie.API.IntegrationEvents.Events
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System;

    public class BulkieStatusChangedToCompleteIntegrationEvent : IntegrationEvent
    {
        public Guid BulkieId { get; set; }
        public string Status { get; set; }

        public BulkieStatusChangedToCompleteIntegrationEvent() { }
    }
}
