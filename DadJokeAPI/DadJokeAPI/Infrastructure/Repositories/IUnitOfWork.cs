using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IJokeRepository JokeRepository { get; }
    }
}
