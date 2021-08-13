using Bulkie.BuildingBlocks.EventBus.Abstractions;
using BulkieFileProcessor.API.IntegrationEvents.Events;
using BulkieFileProcessor.API.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BulkieFileProcessor.API.IntegrationEvents.EventHandlers
{
    public class BulkieFileStatusChangedToSubmittedIntegrationEventHandler : IIntegrationEventHandler<BulkieFileStatusChangedToSubmittedIntegrationEvent>
    {
        private readonly ILogger<BulkieFileStatusChangedToSubmittedIntegrationEventHandler> _logger;
        private readonly IEventBus _eventBus;
        private readonly IFileReferenceRepository _fileReferenceRepository;
        private readonly IBlobRepository _blobRepository;

        public BulkieFileStatusChangedToSubmittedIntegrationEventHandler(ILogger<BulkieFileStatusChangedToSubmittedIntegrationEventHandler> logger, IEventBus eventBus, IFileReferenceRepository fileReferenceRepository, IBlobRepository blobRepository)
        {
            _logger = logger;
            _eventBus = eventBus;
            _fileReferenceRepository = fileReferenceRepository;
            _blobRepository = blobRepository;
        }

        public async Task Handle(BulkieFileStatusChangedToSubmittedIntegrationEvent integrationEvent)
        {
            _logger.LogInformation("----- Handling {name}", nameof(BulkieFileStatusChangedToSubmittedIntegrationEvent));

            var filename = integrationEvent.Filename;
            var filepath = $"Setup/{filename}";

            // Calculate hash of the file and insert it if we don't already have it. 
            var hash = Convert.ToBase64String(await CalculateHash(filepath));

            var fileReference = await _fileReferenceRepository.FindOrCreate(hash);

            var fileExistsInBlobRepository = await _blobRepository.Exists(fileReference.Id.ToString());
            if (!fileExistsInBlobRepository)
            {
                await _blobRepository.Add(fileReference.Id.ToString(), filepath);
            }

            // Many users could upload the same file so 
            // assign the file reference id to the bulkie file
            var @event = new BulkieFileImportedIntegrationEvent()
            {
                BulkieId = integrationEvent.BulkieId,
                BulkieFileId = integrationEvent.BulkieFileId,
                FileReferenceId = fileReference.Id
            };

            await _eventBus.PublishAsync(@event);
        }

        private static async Task<byte[]> CalculateHash(string path)
        {
            using var sha = SHA256.Create();
            using var fileStream = new FileStream(path, FileMode.Open);
            var hash = await sha.ComputeHashAsync(fileStream);

            return hash;
        }
    }
}
