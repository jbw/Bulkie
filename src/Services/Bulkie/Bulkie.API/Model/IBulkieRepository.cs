using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulkie.API.Model
{
    public interface IBulkieRepository
    {
        Task<Bulkie> AddOrGetBulkieAsync(Bulkie readModelBulkie);
        Task<Bulkie> GetBulkieByIdAsync(Guid id);
        Task UpdateBulkieAsync(Bulkie bulkie);
        Task<BulkieSummary> GetBulkieSummaryByIdAsync(Guid id);
        Task<IEnumerable<Bulkie>> GetBulkiesAsync();
        Task<int> GetBulkieFilesCount(Guid id);
        IQueryable<Bulkie> GetBulkieQueryable(Guid id);
    }
}
