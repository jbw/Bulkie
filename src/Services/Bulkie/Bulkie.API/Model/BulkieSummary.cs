using System;

namespace Bulkie.API.Model
{
    public class BulkieSummary
    {
        public Guid Id { get; set; }
        public int TotalFiles { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }
        public string Name { get; set; }
    }
}
