namespace Bulkie.API.IntegrationEvents.Events
{
    using Bulkie.BuildingBlocks.EventBus.Events;
    using System;

    public class BulkieStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        public Guid BulkieId { get; set; }
        public string Status { get; set; }

        public BulkieStatusChangedToSubmittedIntegrationEvent()
        {

        }
    }
}
