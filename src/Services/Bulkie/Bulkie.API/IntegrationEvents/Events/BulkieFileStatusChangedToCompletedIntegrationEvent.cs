namespace Bulkie.API.IntegrationEvents.Events
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System;

    public class BulkieFileStatusChangedToCompletedIntegrationEvent : IntegrationEvent
    {
        public Guid BulkieId { get; set; }
        public Guid BulkieFileId { get; set; }
        public string BulkieFileStatus { get; set; }
        public string BulkieStatus { get; set; }
        public Guid FileReferenceId { get; set; }

        public BulkieFileStatusChangedToCompletedIntegrationEvent() { }
    }
}
