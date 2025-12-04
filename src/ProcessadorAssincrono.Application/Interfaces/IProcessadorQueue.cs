namespace ProcessadorAssincrono.Application.Interfaces
{
    public interface IProcessadorQueue
    {
        Task EnfileirarAsync(Guid id, string pep, string comentariosAdicionais, DateTime utcNow);
    }
}
