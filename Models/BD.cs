namespace TP07_Martinez_Loufer.Models;
using Microsoft.Data.SqlClient;
using Dapper;
public class BD
{
    private string _connectionString =
        @"Server=localhost; Database=DBRedSocial; Integrated Security=True; TrustServerCertificate=True;";

    public int ValidarUsuario(string nombreUsuario, string Contraseña)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            int id = 0;
            string query = "SELECT Id FROM Usuarios WHERE NombreUsuario = @nombreUsuario AND Contraseña = @Contraseña";
            id = connection.QueryFirstOrDefault<int>(query, new { nombreUsuario, Contraseña });
            return id;
        }
    }

    public string BuscarNombreUsuario(string nombreUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT NombreUsuario FROM Usuarios WHERE NombreUsuario = @nombreUsuario";
            string encontrado = connection.QueryFirstOrDefault<string>(query, new { nombreUsuario });
            return encontrado;
        }
    }
    public bool RegistrarUsuario(string nombreUsuario, string Contraseña, string nombre, string apellido)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            if (BuscarNombreUsuario(nombreUsuario) == null)
            {
                string query = "INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido) VALUES (@nombreUsuario, @Contraseña, @nombre, @apellido)";
                connection.Execute(query, new { nombreUsuario, Contraseña, nombre, apellido});
                return true;
            }
            return false;
        }
    }

    public Usuario ObtenerUsuarioPorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Usuarios WHERE Id = @id";
            Usuario usuario = connection.QueryFirstOrDefault<Usuario>(query, new { id });
            return usuario;
        }
    }

    public List<Publicacion> ObtenerPublicacionesRecientes(int cantidad = 10)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT TOP (@cantidad)
                    p.Id,
                    p.IdUsuario AS UsuarioId,
                    u.NombreUsuario AS UsuarioNombre,
                    p.Titulo,
                    p.Descripcion,
                    p.Imagen,
                    p.FechaPublicacion,
                    (SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = p.Id) AS CantidadMeGustas
                FROM Publicaciones p
                INNER JOIN Usuarios u ON u.Id = p.IdUsuario
                ORDER BY p.FechaPublicacion DESC";

            return connection.Query<Publicacion>(query, new { cantidad }).ToList();
        }
    }

    public List<Comentario> ObtenerComentariosPorPublicacion(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT
                    c.Id,
                    c.IdPublicacion,
                    c.IdUsuarioComenta,
                    u.NombreUsuario AS UsuarioNombre,
                    c.Texto,
                    c.FechaComentario
                FROM Comentarios c
                INNER JOIN Usuarios u ON u.Id = c.IdUsuarioComenta
                WHERE c.IdPublicacion = @idPublicacion
                ORDER BY c.FechaComentario ASC";

            return connection.Query<Comentario>(query, new { idPublicacion }).ToList();
        }
    }

    public bool RegistrarLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string verificarQuery = @"SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion AND IdUsuario = @idUsuario";
            int existe = connection.ExecuteScalar<int>(verificarQuery, new { idPublicacion, idUsuario });

            if (existe > 0)
            {
                return false;
            }

            string query = "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@idPublicacion, @idUsuario)";
            int filasAfectadas = connection.Execute(query, new { idPublicacion, idUsuario });
            return filasAfectadas > 0;
        }
    }

    public bool RegistrarComentario(int idPublicacion, int idUsuario, string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario) VALUES (@idPublicacion, @idUsuario, @texto, @fechaComentario)";
            int filasAfectadas = connection.Execute(query, new { idPublicacion, idUsuario, texto, fechaComentario = DateTime.Now });
            return filasAfectadas > 0;
        }
    }

    public bool ActualizarContraseña(int usuarioId, string nuevoContraseña)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Usuarios SET Contraseña = @nuevoContraseña WHERE Id = @usuarioId";
            int filasAfectadas = connection.Execute(query, new { usuarioId, nuevoContraseña });
            return filasAfectadas > 0;
        }
    }

    public bool ExistePublicacion(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(*) FROM Publicaciones WHERE Id = @idPublicacion";
            int cantidad = connection.ExecuteScalar<int>(query, new { idPublicacion });
            return cantidad > 0;
        }
    }

    public bool UsuarioYaDioLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion AND IdUsuario = @idUsuario";
            int cantidad = connection.ExecuteScalar<int>(query, new { idPublicacion, idUsuario });
            return cantidad > 0;
        }
    }

    public (bool liked, int totalLikes) AlternarLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            bool yaTieneLike = UsuarioYaDioLike(idPublicacion, idUsuario);

            if (yaTieneLike)
            {
                string deleteQuery = "DELETE FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion AND IdUsuario = @idUsuario";
                connection.Execute(deleteQuery, new { idPublicacion, idUsuario });
            }
            else
            {
                string insertQuery = "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@idPublicacion, @idUsuario)";
                connection.Execute(insertQuery, new { idPublicacion, idUsuario });
            }

            int totalLikes = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion",
                new { idPublicacion });

            return (liked: !yaTieneLike, totalLikes: totalLikes);
        }
    }

    public Comentario? CrearComentario(int idPublicacion, int idUsuario, string texto)
    {
        if (!ExistePublicacion(idPublicacion) || string.IsNullOrWhiteSpace(texto))
        {
            return null;
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario)
                VALUES (@idPublicacion, @idUsuario, @texto, @fechaComentario);

                SELECT c.Id, c.IdPublicacion, c.IdUsuarioComenta, u.NombreUsuario AS UsuarioNombre,
                       c.Texto, c.FechaComentario
                FROM Comentarios c
                INNER JOIN Usuarios u ON u.Id = c.IdUsuarioComenta
                WHERE c.Id = SCOPE_IDENTITY();";

            var comentario = connection.QuerySingleOrDefault<Comentario>(query, new
            {
                idPublicacion,
                idUsuario,
                texto,
                fechaComentario = DateTime.Now
            });

            return comentario;
        }
    }

    public List<Publicacion> ObtenerPublicacionesDesde(int desde, int cantidad = 10)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT 
                    p.Id,
                    p.IdUsuario AS UsuarioId,
                    u.NombreUsuario AS UsuarioNombre,
                    p.Titulo,
                    p.Descripcion,
                    p.Imagen,
                    p.FechaPublicacion,
                    (SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = p.Id) AS CantidadMeGustas
                FROM Publicaciones p
                INNER JOIN Usuarios u ON u.Id = p.IdUsuario
                ORDER BY p.FechaPublicacion DESC
                OFFSET @desde ROWS
                FETCH NEXT @cantidad ROWS ONLY;";

            return connection.Query<Publicacion>(query, new { desde, cantidad }).ToList();
        }
    }

    public int ObtenerCantidadPublicaciones()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(*) FROM Publicaciones";
            return connection.ExecuteScalar<int>(query);
        }
    }

    public bool CrearPublicacion(int idUsuario, string titulo, string descripcion, string imagen)
    {
        if (idUsuario <= 0 || string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion) || string.IsNullOrWhiteSpace(imagen))
        {
            return false;
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                INSERT INTO Publicaciones (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion)
                VALUES (@idUsuario, @titulo, @descripcion, @imagen, @fechaPublicacion)";

            int filasAfectadas = connection.Execute(query, new
            {
                idUsuario,
                titulo,
                descripcion,
                imagen,
                fechaPublicacion = DateTime.Now
            });

            return filasAfectadas > 0;
        }
    }
}