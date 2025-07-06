using Dapper;

using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Infrastructure.Repositories
{
    public class JokeRepository : IJokeRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<JokeRepository> _logger;

        public JokeRepository(DapperContext context, ILogger<JokeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Joke>> GetJokesByTermAsync(string term)
        {
            using var conn = _context.CreateConnection();

            var rows = await conn.QueryAsync(
                "sp_GetJokesByTerm",
                new { Term = term },
                commandType: CommandType.StoredProcedure);

            var jokes = new List<Joke>();

            foreach (var row in rows)
            {
                var joke = new Joke
                {
                    Id = row.Id,
                    JokeText = row.Joke
                };
                JokeLengthGroup parsedGroup;
                var groupStr = row.Group?.ToString();

                if (Enum.TryParse<JokeLengthGroup>(groupStr, true, out  parsedGroup))
                {
                    joke.Group = parsedGroup;
                }
                else
                {
                    _logger?.LogWarning($"Unknown group '{groupStr}' found in DB. Falling back to Medium.");

                    joke.Group = JokeLengthGroup.Medium;
                }

                jokes.Add(joke);
            }

            return jokes;
        }

        public async Task SaveJokeAsync(Joke joke)
        {
            using var conn = _context.CreateConnection();

            var parameters = new
            {
                Id = joke.Id,
                Joke = joke.JokeText,
                Group = joke.Group.ToString() // enum to string
            };

            await conn.ExecuteAsync(
                "sp_InsertJoke",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
