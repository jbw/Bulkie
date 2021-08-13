using System;

namespace BulkieFileProcessor.API.Model
{
    public class FileReference
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string FileHash { get; set; }
        public DateTime Created { get; set; }

        public FileReference()
        {
            Created = DateTime.UtcNow;
        }
    }
}
