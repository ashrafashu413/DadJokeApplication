using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Web;

namespace Utilities
{
    public class HttpUtility : IHttpUtility
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<HttpUtility> _logger;
        private readonly DadJokeSettings _settings;

        public HttpUtility(IHttpClientFactory factory, ILogger<HttpUtility> logger, IOptions<DadJokeSettings> settings)
        {
            _factory = factory;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<T?> GetAsync<T>(string endpoint, string clientName)
        {
            var client = _factory.CreateClient(clientName);
            client.BaseAddress = new Uri(_settings.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", _settings.UserAgent);

            try
            {
                var response = await client.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Http error {Code} for {Url}", response.StatusCode, endpoint);
                    return default;
                }

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HttpUtility request failed for {Url}", endpoint);
                return default;
            }
        }
    }
}
