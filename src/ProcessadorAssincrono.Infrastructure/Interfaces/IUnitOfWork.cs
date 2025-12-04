namespace ProcessadorAssincrono.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAprovacaoRepository Aprovacoes { get; }
        Task CommitAsync();
        Task RollbackAsync();
    }
}
