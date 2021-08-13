using Dapr.Actors;
using System;
using System.Threading.Tasks;

namespace Bulkie.API.Actors
{
    public interface IBulkieImportActor : IActor
    {
        Task Submit(Guid bulkieId, string name, string[] filenamess);
        Task<Bulkie> GetBulkieDetails();
        Task NotifyBulkieFileImported(Guid bulkieFileId, Guid fileReferenceId);
        Task<bool> Accept();
        Task<bool> Reject();
    }
}
