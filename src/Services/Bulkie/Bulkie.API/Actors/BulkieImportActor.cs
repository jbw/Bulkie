using Bulkie.API.IntegrationEvents.Events;
using Bulkie.BuildingBlocks.EventBus.Abstractions;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bulkie.API.Actors
{

    public class BulkieImportActor : Actor, IBulkieImportActor, IRemindable
    {
        private const string BulkieDetails = "Bulkie";
        private readonly ILogger<BulkieImportActor> _logger;
        private readonly IEventBus _eventBus;

        public BulkieImportActor(ActorHost host, IEventBus eventBus, ILogger<BulkieImportActor> logger) : base(host)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        private Guid BulkieId => Guid.Parse(Id.GetId());

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            throw new NotImplementedException();
        }

        public async Task Submit(Guid bulkieId, string name, string[] filenames)
        {
            _logger.LogInformation("----- Handling {name}", nameof(BulkieImportActor));

            var bulkie = new Bulkie
            {
                Name = name,
                Id = bulkieId,
                BulkieStatus = BulkieStatus.Submitted
            };

            foreach (var filename in filenames)
            {
                bulkie.AddFile(Guid.NewGuid(), filename);
            }

            await StateManager.SetStateAsync(BulkieDetails, bulkie);

            var @event = new BulkieStatusChangedToSubmittedIntegrationEvent()
            {
                BulkieId = bulkieId,
                Status = BulkieStatus.Submitted.Name,
            };

            await _eventBus.PublishAsync(@event);
        }

        public async Task<bool> Accept()
        {
            await TryUpdateBulkieStatusAsync(BulkieStatus.Completed, BulkieStatus.Accepted);

            var bulkie = await StateManager.TryGetStateAsync<Bulkie>(BulkieDetails);

            await _eventBus.PublishAsync(new BulkieStatusChangedToAcceptedIntegrationEvent
            {
                BulkieId = bulkie.Value.Id,
                Status = BulkieStatus.Accepted.Name,
            });

            return true;
        }

        public async Task<bool> Reject()
        {
            await TryUpdateBulkieStatusAsync(BulkieStatus.Completed, BulkieStatus.Rejected);

            var bulkie = await StateManager.TryGetStateAsync<Bulkie>(BulkieDetails);

            await _eventBus.PublishAsync(new BulkieStatusChangedToRejectedIntegrationEvent
            {
                BulkieId = bulkie.Value.Id,
                Status = BulkieStatus.Rejected.Name
            });

            return true;
        }

        public Task<Bulkie> GetBulkieDetails()
        {
            return StateManager.GetStateAsync<Bulkie>(BulkieDetails);
        }

        public async Task NotifyBulkieFileImported(Guid bulkieFileId, Guid fileReferenceId)
        {
            var statusChanged = await TryUpdateBulkieFileStatusAsync(bulkieFileId, BulkieStatus.Submitted, BulkieStatus.Completed);

            if (statusChanged)
            {
                var bulkie = await StateManager.GetStateAsync<Bulkie>(BulkieDetails);

                await _eventBus.PublishAsync(
                    new BulkieFileStatusChangedToCompletedIntegrationEvent
                    {
                        BulkieId = bulkie.Id,
                        BulkieFileId = bulkieFileId,
                        BulkieStatus = bulkie.BulkieStatus.Name, // Keep state changes at our model Bulkie level. We're updating the actor state so pass that to consumers so they don't need to calculate it. 
                        BulkieFileStatus = BulkieStatus.Completed.Name,
                        FileReferenceId = fileReferenceId
                    });
            }
        }

        private async Task<bool> TryUpdateBulkieFileStatusAsync(Guid bulkieFileId, BulkieStatus expectedStatus, BulkieStatus newStatus)
        {
            var bulkie = await StateManager.TryGetStateAsync<Bulkie>(BulkieDetails);

            if (!bulkie.HasValue)
            {
                _logger.LogWarning("Bulkie with Id: {BulkieId} cannot be updated because it doesn't exist",
                    BulkieId);

                return false;
            }

            var bulkieFile = bulkie.Value.BulkieFiles.First(x => x.Id == bulkieFileId);

            if (bulkieFile.BulkieStatus.Id != expectedStatus.Id)
            {
                _logger.LogWarning("BulkieFile with Id: {BulkieFileId} is in status {Status} instead of expected status {ExpectedStatus}",
                    bulkieFileId, bulkieFile.BulkieStatus.Name, expectedStatus.Name);

                return false;
            }

            bulkie.Value.BulkieFiles.First(x => x.Id == bulkieFileId).BulkieStatus = newStatus;

            // Update Bulkie status based on BulkieFile import status
            if (bulkie.Value.BulkieFiles.All(x => x.BulkieStatus == BulkieStatus.Completed))
            {
                bulkie.Value.BulkieStatus = BulkieStatus.Completed;
            }

            await StateManager.SetStateAsync("Bulkie", bulkie.Value);

            return true;
        }

        private async Task<bool> TryUpdateBulkieStatusAsync(BulkieStatus expectedStatus, BulkieStatus newStatus)
        {
            var bulkie = await StateManager.TryGetStateAsync<Bulkie>(BulkieDetails);

            if (!bulkie.HasValue)
            {
                _logger.LogWarning("Bulkie with Id: {BulkieId} cannot be updated because it doesn't exist",
                    BulkieId);

                return false;
            }

            if (bulkie.Value.BulkieStatus.Id != expectedStatus.Id)
            {
                _logger.LogWarning("BulkieFile with Id: {BulkieFileId} is in status {Status} instead of expected status {ExpectedStatus}",
                    bulkie, bulkie.Value.BulkieStatus.Name, expectedStatus.Name);

                return false;
            }

            bulkie.Value.BulkieStatus = newStatus;

            await StateManager.SetStateAsync("Bulkie", bulkie.Value);

            return true;
        }

    }
}
