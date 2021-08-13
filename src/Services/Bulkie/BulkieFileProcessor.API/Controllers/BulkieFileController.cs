using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BulkieFileProcessor.API.Controllers
{
    using BulkieFileProcessor.API.IntegrationEvents.EventHandlers;
    using BulkieFileProcessor.API.IntegrationEvents.Events;
    using Dapr;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    [ApiController]
    [Route("[controller]")]
    public class BulkieFileController : ControllerBase
    {
        private readonly ILogger<BulkieFileController> _logger;
        private const string DaprPubSubName = "pubsub";
        private readonly IServiceProvider _serviceProvider;

        public BulkieFileController(ILogger<BulkieFileController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("BulkieFileStatusChangedToSubmitted")]
        [Topic(DaprPubSubName, nameof(BulkieFileStatusChangedToSubmittedIntegrationEvent))]
        public async Task Handle(BulkieFileStatusChangedToSubmittedIntegrationEvent integrationEvent)
        {
            // Bulkie File submitted lets process each file
            _logger.LogInformation(
                "---- handling {name} {event}", 
                nameof(BulkieFileController), 
                nameof(BulkieFileStatusChangedToSubmittedIntegrationEvent));

            var handler = _serviceProvider.GetRequiredService<BulkieFileStatusChangedToSubmittedIntegrationEventHandler>();       
            await handler.Handle(integrationEvent);
        }
    }
}
