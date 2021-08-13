using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bulkie.API.Controllers
{
    using Bulkie.API.Actors;
    using Bulkie.API.IntegrationEvents.Events;
    using Bulkie.API.Model;
    using Bulkie.BuildingBlocks.EventBus.Abstractions;
    using Dapr;
    using Dapr.Actors;
    using Dapr.Actors.Client;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class UpdateStatusController : ControllerBase
    {
        private readonly ILogger<UpdateStatusController> _logger;
        private readonly IBulkieRepository _bulkieRepository;
        private const string DaprPubSubName = "pubsub";
        private readonly IEventBus _eventBus;
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateStatusController(ILogger<UpdateStatusController> logger, IBulkieRepository bulkieRepository, IEventBus eventBus, IActorProxyFactory actorProxyFactory)
        {
            _logger = logger;
            _bulkieRepository = bulkieRepository;
            _eventBus = eventBus;
            _actorProxyFactory = actorProxyFactory;
        }

        [HttpPost("BulkieSubmitted")]
        [Topic(DaprPubSubName, nameof(BulkieStatusChangedToSubmittedIntegrationEvent))]
        public async Task Handle(BulkieStatusChangedToSubmittedIntegrationEvent integrationEvent)
        {
            _logger.LogInformation("----- Handling {name} with actor id: {actorId}", nameof(BulkieStatusChangedToSubmittedIntegrationEvent), integrationEvent.BulkieId);

            var bulkieProcessor = GetBulkieProcessorActor(integrationEvent.BulkieId);
            var bulkieDetails = await bulkieProcessor.GetBulkieDetails();
            var readModelBulkie = Model.Bulkie.FromActorState(integrationEvent.BulkieId, bulkieDetails);

            // Persist to our Bulkie read model 
            readModelBulkie = await _bulkieRepository.AddOrGetBulkieAsync(readModelBulkie);

            // Publish individual events for each BulkieFile 
            await PublishBulkieFileEvents(bulkieDetails);
        }

        [HttpPost("BulkieCompleted")]
        [Topic(DaprPubSubName, nameof(BulkieStatusChangedToCompleteIntegrationEvent))]
        public async Task Handle(BulkieStatusChangedToCompleteIntegrationEvent integrationEvent)
        {
            // Update our Bulkie read model
            _logger.LogInformation("----- Handling {name} Bulkie Id: {id} and updating status to: {status}", nameof(BulkieStatusChangedToCompleteIntegrationEvent), integrationEvent.BulkieId, integrationEvent.Status);

            await UpdateReadModelAsync(integrationEvent.BulkieId, integrationEvent.Status);
        }

        [HttpPost("BulkieRejected")]
        [Topic(DaprPubSubName, nameof(BulkieStatusChangedToRejectedIntegrationEvent))]
        public Task Handle(BulkieStatusChangedToRejectedIntegrationEvent integrationEvent)
        {
            // Update our Bulkie read model
            _logger.LogInformation("----- Handling {name}", nameof(BulkieStatusChangedToRejectedIntegrationEvent));

            return UpdateReadModelAsync(integrationEvent.BulkieId, integrationEvent.Status);
        }

        [HttpPost("BulkieAccepted")]
        [Topic(DaprPubSubName, nameof(BulkieStatusChangedToAcceptedIntegrationEvent))]
        public Task Handle(BulkieStatusChangedToAcceptedIntegrationEvent integrationEvent)
        {
            // Update our Bulkie read model
            _logger.LogInformation("----- Handling {name}", nameof(BulkieStatusChangedToAcceptedIntegrationEvent));

            return UpdateReadModelAsync(integrationEvent.BulkieId, integrationEvent.Status);
        }

        [HttpPost("BulkieFileCompleted")]
        [Topic(DaprPubSubName, nameof(BulkieFileStatusChangedToCompletedIntegrationEvent))]
        public async Task Handle(BulkieFileStatusChangedToCompletedIntegrationEvent integrationEvent)
        {
            // Update our Bulkie read model
            _logger.LogInformation("----- Handling {name} with bulkie id: {id}", nameof(BulkieFileStatusChangedToCompletedIntegrationEvent), integrationEvent.BulkieId);

            await UpdateReadModelAsync(integrationEvent.BulkieId, integrationEvent.BulkieFileId, integrationEvent.BulkieStatus, integrationEvent.BulkieFileStatus, integrationEvent.FileReferenceId);
        }

        [HttpPost("BulkieFileStatusSubmitted")]
        [Topic(DaprPubSubName, nameof(BulkieFileStatusChangedToSubmittedIntegrationEvent))]
        public async Task Handle(BulkieFileStatusChangedToSubmittedIntegrationEvent integrationEvent)
        {
            // Update our Bulkie read model
            _logger.LogInformation("----- Handling {name} with bulkie id: {id}", nameof(BulkieFileStatusChangedToCompletedIntegrationEvent), integrationEvent.BulkieId);

            await UpdateReadModelAsync(integrationEvent.BulkieId, integrationEvent.BulkieFileId, integrationEvent.BulkieStatus, integrationEvent.BulkieFileStatus);
        }

        private async Task UpdateReadModelAsync(Guid bulkieId, Guid bulkieFileId, string bulkieStatus, string bulkieFileStatus, Guid fileReferenceId=default)
        {
            var bulkie = await _bulkieRepository.GetBulkieByIdAsync(bulkieId);

            if (bulkie != null)
            {
                bulkie.Status = bulkieStatus;

                // Update bulkie completed timestamp
                if (bulkieStatus == BulkieStatus.Completed.Name)
                {
                    bulkie.Completed = DateTime.UtcNow;
                }

                var bulkieFile = bulkie.BulkieFiles.First(x => x.Id == bulkieFileId);
                bulkieFile.Status = bulkieFileStatus;

                // Update bulkie file completed timestamp
                if (bulkieFileStatus == BulkieStatus.Completed.Name)
                {
                    bulkieFile.Completed = DateTime.UtcNow;
                    bulkieFile.FileReferenceId = fileReferenceId;
                }

                _logger.LogInformation("----- UpdateReadModelAsync with bulkie file id: {id} with status: {status}", bulkieFileId, bulkieFileStatus);

                await _bulkieRepository.UpdateBulkieAsync(bulkie);
            }
            else
            {
                _logger.LogWarning("----- Bulkie not found with bulkie id: {id}", bulkieId);
            }
        }

        private async Task UpdateReadModelAsync(Guid bulkieId, string bulkieStatus)
        {
            var bulkie = await _bulkieRepository.GetBulkieByIdAsync(bulkieId);

            if (bulkie != null)
            {
                bulkie.Status = bulkieStatus;

                _logger.LogInformation("----- UpdateReadModelAsync with bulkie id: {id} with status: {status}", bulkie.Id, bulkie.Status);

                await _bulkieRepository.UpdateBulkieAsync(bulkie);
            }
            else
            {
                _logger.LogWarning("----- Bulkie not found with bulkie id: {id}", bulkieId);
            }
        }

        private async Task PublishBulkieFileEvents(Actors.Bulkie bulkie)
        {
            _logger.LogInformation("----- Sending {name} events", nameof(BulkieFileStatusChangedToSubmittedIntegrationEvent));

            foreach (var bulkieFile in bulkie.BulkieFiles)
            {
                _logger.LogInformation("----- Handling {name} for Bulkie: {id} and BulkieFile {fileId}",
                    nameof(BulkieFileStatusChangedToSubmittedIntegrationEvent),
                    bulkie.Id,
                    bulkieFile.Id);

                var bulkieFileEvent = new BulkieFileStatusChangedToSubmittedIntegrationEvent()
                {
                    BulkieId = bulkie.Id,
                    BulkieFileId = bulkieFile.Id,
                    BulkieFileStatus = BulkieStatus.Submitted.Name,
                    BulkieStatus = bulkie.BulkieStatus.Name,
                    Filename = bulkieFile.Filename
                };

                await _eventBus.PublishAsync(bulkieFileEvent);
            }
        }

        private IBulkieImportActor GetBulkieProcessorActor(Guid id)
        {
            var actorId = new ActorId(id.ToString());
            var bulkieProcessor = _actorProxyFactory.CreateActorProxy<IBulkieImportActor>(actorId, nameof(BulkieImportActor));

            return bulkieProcessor;
        }
    }
}
