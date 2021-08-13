using System;

namespace Bulkie.API.Model
{
    public class BulkieFileDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }
        public string Filename { get; set; }
        public Guid FileReferenceId { get; set; }

        public static BulkieFileDto FromBulkieFile(BulkieFile bulkieFile)
        {
            return new BulkieFileDto
            {
                Id = bulkieFile.Id,
                Status = bulkieFile.Status,
                Filename = bulkieFile.Filename,
                FileReferenceId = bulkieFile.FileReferenceId,
                Completed = bulkieFile.Completed,
                Created = bulkieFile.Created
            };
        }
    }
}
