using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IJokeRepository JokeRepository { get; }

        public UnitOfWork(IJokeRepository jokeRepository)
        {
            JokeRepository = jokeRepository;
        }
    }
}
