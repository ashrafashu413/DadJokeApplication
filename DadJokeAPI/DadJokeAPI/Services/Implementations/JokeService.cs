
using Domain.Settings;
using Infrastructure.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Services.Interfaces;
using Utilities;

namespace Services.Implementations
{
    public class JokeService : IJokeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpUtility _http;
        private readonly IMemoryCache _cache;
        private readonly DadJokeSettings _settings;
        private readonly ILogger<JokeService> _logger;
        private readonly IElasticClient _elasticClient;

        public JokeService(
            IUnitOfWork uow,
            IHttpUtility http,
            IMemoryCache cache,
            IOptions<DadJokeSettings> settings,
            ILogger<JokeService> logger,
            IElasticClient elasticClient)
        {
            _uow = uow;
            _http = http;
            _cache = cache;
            _settings = settings.Value;
            _logger = logger;
            _elasticClient = elasticClient;
        }

        public async Task<Joke> GetRandomJokeAsync()
        {
            var joke = await _http.GetAsync<Joke>("", "DadJokeClient");

            if (joke == null || string.IsNullOrWhiteSpace(joke.JokeText))
            {
                _logger?.LogWarning("Random joke fetch failed or joke is null/empty.");
                return null;
            }

            joke.Group = JokeLengthClassifier.Classify(joke.JokeText);

            try
            {
                await _uow.JokeRepository.SaveJokeAsync(joke);
                await IndexJokeInElasticAsync(joke);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving random joke to DB/ES.");
            }

            return joke;
        }

        public async Task<List<JokeGroup>> SearchJokesAsync(string term)
        {
            if (_cache.TryGetValue(term, out List<JokeGroup> cached))
            {
                _logger?.LogInformation("Cache hit for search term: {Term}", term);
                return cached;
            }

            List<Joke> jokes = new();

            // ✅ 1. Try Elasticsearch
            try
            {
                var esResponse = await _elasticClient.SearchAsync<Joke>(s => s
                    .Index("jokes")
                    .Size(_settings.SearchLimit)
                    .Query(q => q.Match(m => m.Field(f => f.JokeText).Query(term)))
                );

                if (esResponse.IsValid && esResponse.Documents.Any())
                {
                    _logger?.LogInformation("Found {Count} jokes in Elasticsearch.", esResponse.Documents.Count);
                    jokes = esResponse.Documents.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Elasticsearch query failed. Falling back to DB.");
            }

            // ✅ 2. Try SQL DB
            if (!jokes.Any())
            {
                jokes = (await _uow.JokeRepository.GetJokesByTermAsync(term)).ToList();

                if (jokes.Any())
                {
                    _logger?.LogInformation("Found {Count} jokes in SQL DB.", jokes.Count);
                }
            }

            // ✅ 3. Try Dad Joke API
            if (!jokes.Any())
            {
                _logger?.LogInformation("Fetching jokes from API for term: {Term}", term);

                var response = await _http.GetAsync<JokeSearchResponse>(
                    $"/search?term={term}&limit={_settings.SearchLimit}",
                    "DadJokeClient");

                jokes = response?.Results?.Select(j =>
                {
                    j.JokeText = TextHighlighter.EmphasizeTerm(j.JokeText, term);
                    j.Group = JokeLengthClassifier.Classify(j.JokeText);
                    return j;
                }).ToList() ?? new();

                foreach (var joke in jokes)
                {
                    try
                    {
                        await _uow.JokeRepository.SaveJokeAsync(joke);
                        await IndexJokeInElasticAsync(joke);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed saving joke ID {Id} to DB/ES", joke.Id);
                    }
                }
            }

            // ✅ 4. Group & Cache
            var grouped = jokes
                .GroupBy(j => j.Group)
                .Select(g => new JokeGroup
                {
                    Group = g.Key.ToString(),
                    Jokes = g.ToList()
                }).ToList();

            _cache.Set(term, grouped, TimeSpan.FromMinutes(_settings.CacheDurationMinutes));

            return grouped;
        }

        private async Task IndexJokeInElasticAsync(Joke joke)
        {
            try
            {
                var response = await _elasticClient.IndexAsync(joke, i => i.Index("jokes").Id(joke.Id));
                if (!response.IsValid)
                    _logger?.LogWarning("Failed to index joke {Id} in ES: {Reason}", joke.Id, response.ServerError?.ToString());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to index joke in Elasticsearch.");
            }
        }

        private class JokeSearchResponse
        {
            public List<Joke> Results { get; set; }
        }
    }
}
