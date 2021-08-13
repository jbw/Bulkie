using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bulkie.API.Controllers
{
    using Bulkie.API.Actors;
    using Bulkie.API.Model;
    using Bulkie.API.ViewModel;
    using Bulkie.BuildingBlocks.EventBus.Abstractions;
    using Dapr.Actors;
    using Dapr.Actors.Client;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Annotations;
    using System.Collections.Generic;
    using System.Linq;
    using static Bulkie.API.ViewModel.BulkieController;

    [ApiController]
    [Route("[controller]")]
    public partial class BulkieController : ControllerBase
    {
        private readonly ILogger<BulkieController> _logger;
        private readonly IEventBus _eventBus;
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IBulkieRepository _bulkieRepository;

        public BulkieController(ILogger<BulkieController> logger, IEventBus eventBus, IActorProxyFactory actorProxyFactory, IBulkieRepository bulkieRepository)
        {
            _logger = logger;
            _eventBus = eventBus;
            _actorProxyFactory = actorProxyFactory;
            _bulkieRepository = bulkieRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BulkieDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBulkiesAsync()
        {
            _logger.LogInformation("----- Handling {name}", nameof(GetBulkiesAsync));

            var bulkies = await _bulkieRepository.GetBulkiesAsync();

            if (bulkies == null) return NotFound();

            return Ok(bulkies.Select(BulkieDto.FromBulkie));
        }

        [Route("{bulkieId:Guid}/summary")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BulkieSummaryDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBulkieSummaryAsync(Guid bulkieId)
        {
            _logger.LogInformation("----- Handling {name}", nameof(GetBulkieSummaryAsync));

            var bulkie = await _bulkieRepository.GetBulkieSummaryByIdAsync(bulkieId);

            if (bulkie == null) return NotFound(bulkieId);

            return Ok(BulkieSummaryDto.FromBulkieSummary(bulkie));
        }

        [Route("{bulkieId:Guid}/files")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BulkieFileDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBulkieFilesListAsync(Guid bulkieId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            _logger.LogInformation("----- Handling {name}", nameof(GetBulkieFilesListAsync));

            var bulkie = await _bulkieRepository.GetBulkieByIdAsync(bulkieId);

            if (bulkie == null) return NotFound(bulkieId);

            var totalItems = await _bulkieRepository.GetBulkieFilesCount(bulkieId);

            var bulkieFilesPaged = await _bulkieRepository
                .GetBulkieQueryable(bulkieId)
                .SelectMany(x => x.BulkieFiles)
                .OrderBy(c => c)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var bulkieFilesPagedDto = bulkieFilesPaged.Select(BulkieFileDto.FromBulkieFile);

            var model = new PaginatedItemsViewModel<BulkieFileDto>(pageIndex, pageSize, totalItems, bulkieFilesPagedDto);

            return Ok(model);
        }

        [Route("{bulkieId:Guid}/files/{bulkieFileId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(BulkieFileDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBulkieFileAsync(Guid bulkieId, Guid bulkieFileId)
        {
            _logger.LogInformation("----- Handling {name}", nameof(GetBulkieFileAsync));

            var bulkie = await _bulkieRepository.GetBulkieByIdAsync(bulkieId);

            if (bulkie == null) return NotFound(bulkieId);

            var bulkieFile = bulkie.BulkieFiles.SingleOrDefault(x => x.Id == bulkieFileId);

            if (bulkieFile == null) return NotFound(bulkieId);

            return Ok(BulkieFileDto.FromBulkieFile(bulkieFile));

        }

        [Route("{bulkieId:Guid}/reject")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RejectBulkieAsync(Guid bulkieId)
        {
            _logger.LogInformation("----- Handling {name}", nameof(RejectBulkieAsync));

            var bulkieProcessor = GetBulkieProcessorActor(bulkieId);
            var isRejected = await bulkieProcessor.Reject();

            return Ok(new { isRejected });
        }

        [Route("{bulkieId:Guid}/accept")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CompleteBulkieAsync(Guid bulkieId)
        {
            _logger.LogInformation("----- Handling {name}", nameof(CompleteBulkieAsync));

            var bulkieProcessor = GetBulkieProcessorActor(bulkieId);
            var isAccepted = await bulkieProcessor.Accept();

            return Ok(new { isAccepted });
        }


        [SwaggerOperation(
            Summary = "Import files from the Setup folder.",
            Description = "For simplicity, manually add files you want to import to the Setup/ directory in BulkieFileProcessor.API"
        )]
        [Route("/submit")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SubmitBulkieAsync(BulkieViewModel bulkieViewModel)
        {
            _logger.LogInformation("----- Handling {name}", nameof(SubmitBulkieAsync));

            var bulkieId = Guid.NewGuid();
            var bulkieProcessor = GetBulkieProcessorActor(bulkieId);
            await bulkieProcessor.Submit(bulkieId, bulkieViewModel.Name, bulkieViewModel.Filenames);

            return Ok(bulkieId);
        }

        private IBulkieImportActor GetBulkieProcessorActor(Guid id)
        {
            var actorId = new ActorId(id.ToString());
            var bulkieProcessor = _actorProxyFactory.CreateActorProxy<IBulkieImportActor>(actorId, nameof(BulkieImportActor));
            return bulkieProcessor;
        }
    }
}
