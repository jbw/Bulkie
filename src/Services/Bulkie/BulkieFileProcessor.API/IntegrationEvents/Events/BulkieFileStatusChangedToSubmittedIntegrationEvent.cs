namespace BulkieFileProcessor.API.IntegrationEvents.Events
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System;

    public class BulkieFileStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        public Guid BulkieId { get; set; }
        public Guid BulkieFileId { get; set; }
        public string Filename { get; set; }
        public BulkieFileStatusChangedToSubmittedIntegrationEvent() { }
    }
}
