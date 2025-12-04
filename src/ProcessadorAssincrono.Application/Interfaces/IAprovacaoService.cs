using ProcessadorAssincrono.Domain.Entities;

namespace ProcessadorAssincrono.Application.Interfaces
{
    public interface IAprovacaoService
    {
        Task AprovarAsync(Guid id, string pep, string comentariosAdicionais, DateTime dataAprovacao);

        Task InserirAsync(Aprovacao aprovacao);
    }
}
