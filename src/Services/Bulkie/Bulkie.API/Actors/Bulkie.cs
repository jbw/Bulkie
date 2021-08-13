using System;
using System.Collections.Generic;

namespace Bulkie.API.Actors
{

    public class Bulkie
    {
        public Guid Id { get; set; }
        public BulkieStatus BulkieStatus { get; set; }
        public IList<BulkieFile> BulkieFiles { get; set; }
        public string Name { get; set; }

        public Bulkie()
        {
            BulkieFiles = new List<BulkieFile>();
        }

        public void AddFile(Guid id, string filename)
        {
            BulkieFiles.Add(new BulkieFile(id) { Filename = filename });
        }
    }
}
