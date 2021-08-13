using System;
using System.Collections.Generic;
using System.Linq;

namespace Bulkie.API.Model
{

    public class Bulkie
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public IList<BulkieFile> BulkieFiles { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }

        public Bulkie()
        {
            Created = DateTime.UtcNow;
        }

        internal static Bulkie FromActorState(Guid bulkieId, Actors.Bulkie bulkieDetails)
        {
            return new Bulkie
            {
                Id = bulkieId,
                Name = bulkieDetails.Name,
                Status = bulkieDetails.BulkieStatus.Name,
                BulkieFiles = bulkieDetails.BulkieFiles
                    .Select(BulkieFile.FromActorState)
                    .ToList()
            };
        }
    }
}
