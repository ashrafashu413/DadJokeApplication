

namespace Infrastructure.Repositories
{
    public interface IJokeRepository
    {
        Task<IEnumerable<Joke>> GetJokesByTermAsync(string term);
        Task SaveJokeAsync(Joke joke);
    }
}
