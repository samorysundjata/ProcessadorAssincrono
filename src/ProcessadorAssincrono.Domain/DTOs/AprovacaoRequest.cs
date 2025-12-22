namespace ProcessadorAssincrono.Domain.DTOs
{
    public record AprovacaoRequest(string Projeto, string ComentariosAdicionais, DateTime DataAprovacao);
}
