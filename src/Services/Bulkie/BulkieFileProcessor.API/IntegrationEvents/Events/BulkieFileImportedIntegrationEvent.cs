namespace BulkieFileProcessor.API.IntegrationEvents.Events
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System;

    public class BulkieFileImportedIntegrationEvent : IntegrationEvent
    {
        public Guid BulkieId { get; set; }
        public Guid BulkieFileId { get; set; }
        public Guid FileReferenceId { get; set; }

        public BulkieFileImportedIntegrationEvent()
        {

        }
    }
}
