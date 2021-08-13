using Bulkie.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulkie.API.Model
{
    public class BulkieRepository : IBulkieRepository
    {
        private readonly BulkieContext _bulkieContext;

        public BulkieRepository(BulkieContext bulkieContext)
        {
            _bulkieContext = bulkieContext ?? throw new ArgumentNullException(nameof(bulkieContext));
        }

        public async Task<Bulkie> AddOrGetBulkieAsync(Bulkie bulkie)
        {
            _bulkieContext.Bulkies.Add(bulkie);

            try
            {
                await _bulkieContext.SaveChangesAsync();
                return bulkie;
            }
            catch (DbUpdateException)
            {
                return await GetBulkieByIdAsync(bulkie.Id);
            }
        }

        public async Task<Bulkie> GetBulkieByIdAsync(Guid id)
        {
            var bulkie = await _bulkieContext.Bulkies
                .Where(o => o.Id == id)
                .Include(o => o.BulkieFiles)
                .SingleOrDefaultAsync();

            return bulkie;
        }

        public async Task<int> GetBulkieFilesCount(Guid id)
        {
            return await _bulkieContext.BulkieFiles.Where(x => x.Bulkie.Id == id).CountAsync();
        }

        public IQueryable<Bulkie> GetBulkieQueryable(Guid id)
        {
            return _bulkieContext.Bulkies.Where(o => o.Id == id).AsQueryable();
        }

        public async Task<IEnumerable<Bulkie>> GetBulkiesAsync()
        {
            return await _bulkieContext.Bulkies.ToListAsync();
        }

        public async Task<BulkieSummary> GetBulkieSummaryByIdAsync(Guid id)
        {
            return await _bulkieContext.Bulkies
                            .Where(o => o.Id == id)
                            .Include(o => o.BulkieFiles)
                            .Select(o => new BulkieSummary
                            {
                                Id = o.Id,
                                Name = o.Name,
                                Completed = o.Completed,
                                Created = o.Created,
                                Status = o.Status,
                                TotalFiles = o.BulkieFiles.Count,
                                Duration = o.Completed.Subtract(o.Created).ToString("g")
                            })
                            .SingleOrDefaultAsync();
        }

        public async Task UpdateBulkieAsync(Bulkie bulkie)
        {
            _bulkieContext.Update(bulkie);

            await _bulkieContext.SaveChangesAsync();
        }
    }
}
