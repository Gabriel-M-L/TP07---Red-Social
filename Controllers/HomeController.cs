using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP07_Martinez_Loufer.Models;

namespace TP07_Martinez_Loufer.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("id") == null)
        {
            return RedirectToAction("Login");
        }

        int id = int.Parse(HttpContext.Session.GetString("id"));
        BD bd = new BD();
        ViewBag.usuario = bd.ObtenerUsuarioPorId(id);

        List<Publicacion> publicaciones = bd.ObtenerPublicacionesRecientes(10);
        foreach (Publicacion publicacion in publicaciones)
        {
            publicacion.MeGustaUsuario = bd.UsuarioYaDioLike(publicacion.Id, id);
            publicacion.Comentarios = bd.ObtenerComentariosPorPublicacion(publicacion.Id);
        }

        ViewBag.HayMasPublicaciones = bd.ObtenerCantidadPublicaciones() > publicaciones.Count;

        return View(publicaciones);
    }

    [HttpPost]
    public IActionResult ToggleLike(int publicacionId)
    {
        if (HttpContext.Session.GetString("id") == null)
        {
            return Json(new { success = false, message = "Debe iniciar sesión" });
        }

        int usuarioId = int.Parse(HttpContext.Session.GetString("id"));
        BD bd = new BD();

        if (!bd.ExistePublicacion(publicacionId))
        {
            return Json(new { success = false, message = "La publicación no existe" });
        }

        var resultado = bd.AlternarLike(publicacionId, usuarioId);

        return Json(new
        {
            success = true,
            liked = resultado.liked,
            likes = resultado.totalLikes
        });
    }

    [HttpPost]
    public IActionResult AgregarComentario(int publicacionId, string texto)
    {
        if (HttpContext.Session.GetString("id") == null)
        {
            return Json(new { success = false, message = "Debe iniciar sesión" });
        }

        int usuarioId = int.Parse(HttpContext.Session.GetString("id"));
        BD bd = new BD();

        if (!bd.ExistePublicacion(publicacionId))
        {
            return Json(new { success = false, message = "La publicación no existe" });
        }

        if (string.IsNullOrWhiteSpace(texto))
        {
            return Json(new { success = false, message = "El comentario no puede estar vacío" });
        }

        Comentario? comentario = bd.CrearComentario(publicacionId, usuarioId, texto.Trim());
        if (comentario == null)
        {
            return Json(new { success = false, message = "No se pudo guardar el comentario" });
        }

        return Json(new
        {
            success = true,
            comentario = new
            {
                id = comentario.Id,
                usuarioNombre = comentario.UsuarioNombre,
                texto = comentario.Texto,
                fechaComentario = comentario.FechaComentario.ToString("dd/MM/yyyy HH:mm")
            }
        });
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    public IActionResult RegistroUsuario(string nombreUsuario, string Contraseña, string nombre, string apellido, string tipoUsuario, string email)
    {
        BD bd = new BD();
        if(bd.RegistrarUsuario(nombreUsuario, Contraseña, nombre, apellido))
        {
            return RedirectToAction("Login");
        }
        return RedirectToAction("Registro", new { mensajeError = "Ese usuario ya existe" });
    }
    [HttpPost]
    public IActionResult VerificarSesion(string nombreUsuario, string Contraseña)
    {
        BD bd = new BD();
        int id = bd.ValidarUsuario(nombreUsuario, Contraseña);
        if (id == 0)
        {
           return RedirectToAction("Login", new { mensajeError = "Usuario o contraseña incorrectos" }); 
        }
        HttpContext.Session.SetString("id", id.ToString());
        return RedirectToAction("Index");
    }

    public IActionResult Login(string mensajeError, string mensajeExito)
    {
        ViewBag.MensajeError = mensajeError;
        ViewBag.MensajeExito = mensajeExito;
        return View();
    }

    public IActionResult Registro(string mensajeError)
    {
        ViewBag.MensajeError = mensajeError;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult ObtenerMas(int desde)
    {
        if (HttpContext.Session.GetString("id") == null)
        {
            return Json(new { success = false, message = "Debe iniciar sesión" });
        }

        int usuarioId = int.Parse(HttpContext.Session.GetString("id"));
        BD bd = new BD();

        List<Publicacion> publicaciones = bd.ObtenerPublicacionesDesde(desde, 10);
        foreach (Publicacion publicacion in publicaciones)
        {
            publicacion.MeGustaUsuario = bd.UsuarioYaDioLike(publicacion.Id, usuarioId);
            publicacion.Comentarios = bd.ObtenerComentariosPorPublicacion(publicacion.Id);
        }

        var resultado = publicaciones.Select(p => new
        {
            id = p.Id,
            titulo = p.Titulo,
            descripcion = p.Descripcion,
            imagen = string.IsNullOrWhiteSpace(p.Imagen)
                ? $"https://placehold.co/900x500?text={Uri.EscapeDataString(p.Titulo)}"
                : $"/images/{p.Imagen}",
            usuarioNombre = p.UsuarioNombre,
            fechaPublicacion = p.FechaPublicacion.ToString("dd/MM/yyyy HH:mm"),
            cantidadMeGustas = p.CantidadMeGustas,
            meGustaUsuario = p.MeGustaUsuario,
            comentarios = p.Comentarios.Select(c => new
            {
                usuarioNombre = c.UsuarioNombre,
                texto = c.Texto,
                fechaComentario = c.FechaComentario.ToString("dd/MM/yyyy HH:mm")
            }).ToList()
        }).ToList();

        return Json(new { success = true, publicaciones = resultado });
    }

    [HttpPost]
    public IActionResult CrearPublicacion(string titulo, string descripcion, string imagen)
    {
        if (HttpContext.Session.GetString("id") == null)
        {
            return RedirectToAction("Login");
        }

        int idUsuario = int.Parse(HttpContext.Session.GetString("id"));
        BD bd = new BD();

        if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion) || string.IsNullOrWhiteSpace(imagen))
        {
            return RedirectToAction("Index", new { mensajeError = "Todos los campos son obligatorios" });
        }

        bd.CrearPublicacion(idUsuario, titulo.Trim(), descripcion.Trim(), imagen.Trim());
        return RedirectToAction("Index");
    }
}
