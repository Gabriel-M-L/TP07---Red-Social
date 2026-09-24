namespace TP07_Martinez_Loufer.Models;
public class Publicacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Imagen { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public int CantidadMeGustas { get; set; }
    public bool MeGustaUsuario { get; set; }
    public List<Comentario> Comentarios { get; set; } = new();
}