using System;

namespace Bulkie.API.Model
{
    public class BulkieFile
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Bulkie Bulkie { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }
        public string Filename { get; set; }
        public Guid FileReferenceId { get; set; }

        public BulkieFile()
        {
            Created = DateTime.UtcNow;
        }

        internal static BulkieFile FromActorState(Actors.BulkieFile bulkieFile)
        {
            return new BulkieFile
            {
                Id = bulkieFile.Id,
                Status = "Pending", 
                Filename = bulkieFile.Filename
            };
        }
    }
}
