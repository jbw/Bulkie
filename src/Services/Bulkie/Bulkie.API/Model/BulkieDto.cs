using System;

namespace Bulkie.API.Model
{
    public class BulkieDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }

        public static BulkieDto FromBulkie(Bulkie bulkie)
        {
            return new BulkieDto
            {
                Id = bulkie.Id,
                Name = bulkie.Name,
                Created = bulkie.Created,
                Completed = bulkie.Completed,
                Status = bulkie.Status,
            };
        }
    }
}
