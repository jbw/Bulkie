namespace Bulkie.API.Actors
{
    public class BulkieStatus
    {
        public static readonly BulkieStatus Submitted = new(1, nameof(Submitted));
        public static readonly BulkieStatus Rejected = new(3, nameof(Rejected));
        public static readonly BulkieStatus Accepted = new(4, nameof(Accepted));
        public static readonly BulkieStatus Completed = new(2, nameof(Completed));

        public BulkieStatus()
        {
        }

        public BulkieStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
