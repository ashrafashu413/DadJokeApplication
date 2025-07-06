

namespace Services.Interfaces
{
    public interface IJokeService
    {
        Task<Joke> GetRandomJokeAsync();
        Task<List<JokeGroup>> SearchJokesAsync(string term);
    }
}
