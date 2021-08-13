using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bulkie.API.Controllers
{
    using Bulkie.API.Actors;
    using Bulkie.API.IntegrationEvents.Events;
    using Dapr;
    using Dapr.Actors;
    using Dapr.Actors.Client;
    using Microsoft.Extensions.Logging;
    using System;

    [ApiController]
    [Route("[controller]")]
    public class BulkieProcessEventController : ControllerBase
    {
        private readonly ILogger<UpdateStatusController> _logger;
        private readonly IActorProxyFactory _actorProxyFactory;

        private const string DaprPubSubName = "pubsub";

        public BulkieProcessEventController(ILogger<UpdateStatusController> logger, IActorProxyFactory actorProxyFactory)
        {
            _logger = logger;
            _actorProxyFactory = actorProxyFactory;
        }

        [HttpPost("BulkieFileImported")]
        [Topic(DaprPubSubName, nameof(BulkieFileImportedIntegrationEvent))]
        public async Task Handle(BulkieFileImportedIntegrationEvent integrationEvent)
        {
            await GetBulkieProcessorActor(integrationEvent.BulkieId)
                .NotifyBulkieFileImported(integrationEvent.BulkieFileId, integrationEvent.FileReferenceId);
        }

        private IBulkieImportActor GetBulkieProcessorActor(Guid id)
        {
            var actorId = new ActorId(id.ToString());
            var bulkieProcessor = _actorProxyFactory.CreateActorProxy<IBulkieImportActor>(actorId, nameof(BulkieImportActor));
            return bulkieProcessor;
        }
    }
}
