using System;

namespace Bulkie.API.Model
{
    public class BulkieSummaryDto
    {
        public Guid Id { get; set; }
        public int TotalFiles { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }
        public string Name { get; set; }

        public static BulkieSummaryDto FromBulkieSummary(BulkieSummary summary)
        {
            return new BulkieSummaryDto
            {
                Id = summary.Id,
                Name = summary.Name,
                Created = summary.Created,
                Completed = summary.Completed,
                Status = summary.Status,
                TotalFiles = summary.TotalFiles,
                Duration = summary.Duration
            };
        }
    }
}
