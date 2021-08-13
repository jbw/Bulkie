using System;

namespace Bulkie.API.Actors
{
    public class BulkieFile
    {
        public Guid Id { get; set; }
        public BulkieStatus BulkieStatus { get; set; }
        public string Filename { get; set; }

        public BulkieFile()
        {

        }

        public BulkieFile(Guid id)
        {
            Id = id;
            BulkieStatus = BulkieStatus.Submitted;
        }
    }
}
