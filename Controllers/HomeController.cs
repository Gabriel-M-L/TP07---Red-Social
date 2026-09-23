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
        return View();
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    public IActionResult RegistroUsuario(string nombreUsuario, string contraseña, string nombre, string apellido, string tipoUsuario, string email)
    {
        BD bd = new BD();
        if(bd.RegistrarUsuario(nombreUsuario, contraseña, nombre, apellido))
        {
            return RedirectToAction("Login");
        }
        return RedirectToAction("Registro", new { mensajeError = "Ese usuario ya existe" });
    }
    [HttpPost]
    public IActionResult VerificarSesion(string nombreUsuario, string contraseña)
    {
        BD bd = new BD();
        int id = bd.ValidarUsuario(nombreUsuario, contraseña);
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
}
