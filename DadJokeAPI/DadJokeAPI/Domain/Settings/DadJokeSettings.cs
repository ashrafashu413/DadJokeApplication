namespace Domain.Settings
{
    public class DadJokeSettings
    {
        public string BaseUrl { get; set; }
        public string UserAgent { get; set; }
        public int CacheDurationMinutes { get; set; }
        public int SearchLimit { get; set; }
    }

    public class ElasticSearchSettings
    {
        public string Uri { get; set; }
        public string DefaultIndex { get; set; }
    }
}
