namespace Utilities
{
    public interface IHttpUtility
    {
        Task<T?> GetAsync<T>(string endpoint, string clientName);
    }
}
