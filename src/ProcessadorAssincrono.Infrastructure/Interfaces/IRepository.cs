namespace ProcessadorAssincrono.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> ObterPorId(Guid id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task InserirAsync(T entity);
    }
}
