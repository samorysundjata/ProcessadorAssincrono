namespace ProcessadorAssincrono.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAprovacaoRepository Aprovacoes { get; }
        Task CommitAsync();
        Task RollbackAsync();
    }
}
