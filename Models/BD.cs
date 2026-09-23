namespace TP07_Martinez_Loufer.Models;
using Microsoft.Data.SqlClient;
using Dapper;
public class BD
{
    private string _connectionString =
        @"Server=localhost; DataBase=DBRedSocial ; IntegratedSecurity=True; TrustServerCertificate=True;";

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

    public bool ActualizarContraseña(int usuarioId, string nuevoContraseña)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Usuarios SET Contraseña = @nuevoContraseña WHERE Id = @usuarioId";
            int filasAfectadas = connection.Execute(query, new { usuarioId, nuevoContraseña });
            return filasAfectadas > 0;
        }
    }
}