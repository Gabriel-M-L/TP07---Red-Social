namespace TP07_Martinez_Loufer.Models;
public class Publicacion
{
    public int Id { get; set; }
    public string Imagen { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaPublicacion { get; set; }
}