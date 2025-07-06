namespace Domain.Constants
{
    public static class AppConstants
    {
        public static class Clients
        {
            public const string DadJokeClient = "DadJokeClient";
        }

        public static class CacheKeys
        {
            public static string GetJokeSearchKey(string term) => $"Jokes_Search_{term}";
        }

        public static class Config
        {
            public const string DadJokeSettingsSection = "DadJokeSettings";
        }
    }
}
