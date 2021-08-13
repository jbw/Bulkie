namespace BulkieFileProcessor.API.IntegrationEvents.Events
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System;

    public class BulkieFileStatusChangedToCompletedIntegrationEvent : IntegrationEvent
    {
        public Guid BulkieId { get; set; }
        public Guid BulkieFileId { get; set; }
        public string Status { get; set; }

        public BulkieFileStatusChangedToCompletedIntegrationEvent() { }
    }
}
