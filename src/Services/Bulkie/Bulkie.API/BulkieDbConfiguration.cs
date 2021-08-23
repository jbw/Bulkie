namespace Bulkie.API
{
    public class BulkieDbConfiguration
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string ToConnectionString()
        {
            return $"host={Host};database={Database};user id={Username};password={Password}";
        }
    }
}
