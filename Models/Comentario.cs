namespace TP07_Martinez_Loufer.Models;

public class Comentario
{
    public int Id { get; set; }
    public int IdPublicacion { get; set; }
    public int IdUsuarioComenta { get; set; }
    public string Texto { get; set; }
    public DateTime FechaComentario { get; set; }
}