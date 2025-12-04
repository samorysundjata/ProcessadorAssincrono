namespace ProcessadorAssincrono.Domain.Entities
{
    public class Aprovacao
    {
        public Guid Id { get; set; }
        public string Pep { get; set; } = string.Empty;
        public string ComentariosAdicionais { get; set; } = string.Empty;
        public DateTime DataAprovacao { get; set; } = DateTime.UtcNow;
    }
}
