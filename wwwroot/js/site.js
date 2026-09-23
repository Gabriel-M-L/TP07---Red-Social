function VerificarSesion() {
    var password = document.getElementById("password").value;
    var error = document.getElementById("error");
    var cuentaError = 0;
    if (password.length < 8) {
        error.innerHTML += "\nLa contraseña debe tener al menos 8 caracteres.";
        error.style.color = "red";
        cuentaError++;
    }
    if (cuentaError > 0) {
        return false;
    }
    return true;
}

function VerificarRegistro() {
    var password = document.getElementById("password").value;
    var nombre = document.getElementById("nombre").value;
    var apellido = document.getElementById("apellido").value;
    var error = document.getElementById("error");
    var cuentaError = 0;
    var i = 0;
    const caracteresRegulares = /^[A-Za-z]$/;
    error.innerHTML = "";

    if (password.length < 8) {
        error.innerHTML += "\nLa contraseña no es suficientemente segura. Debe tener al menos 8 caracteres.";
        error.style.color = "red";
        cuentaError++;
    }
    while (i < nombre.length && caracteresRegulares.test(nombre[i])) {
        i++;
    }
    if (i < nombre.length) {
        error.innerHTML += "\nEl nombre no puede contener caracteres especiales.";
        error.style.color = "red";
        cuentaError++;
    }
    i = 0;
    while (i < apellido.length && caracteresRegulares.test(apellido[i])) {
        i++;
    }
    if (i < apellido.length) {
        error.innerHTML += "\nEl apellido no puede contener caracteres especiales.";
        error.style.color = "red";
        cuentaError++;
    }
    if (cuentaError > 0) {
        return false;
    }
    return true;
}
function ValidarContrasenas() {

    var nuevoPassword = document.getElementById("nuevoPassword").value;
    var confirmarPassword = document.getElementById("confirmarPassword").value;
    var errorDiv = document.getElementById("errorPassword");
    var cuentaError = 0;
    errorDiv.innerHTML = "";

    if (nuevoPassword.length < 8) {
        errorDiv.innerHTML += "\nLa contraseña debe tener al menos 8 caracteres.";
        errorDiv.style.color = "red";
        cuentaError++;
    }

    if (confirmarPassword.length < 8) {
        errorDiv.innerHTML += "\nLa confirmación de contraseña debe tener al menos 8 caracteres.";
        errorDiv.style.color = "red";
        cuentaError++;
    }

    if (nuevoPassword !== confirmarPassword) {
        errorDiv.innerHTML += "\nLas contraseñas no coinciden.";
        errorDiv.style.color = "red";
        cuentaError++;
    }

    if (cuentaError > 0) {
        errorDiv.style.display = "block";
        return false;
    }

    errorDiv.style.display = "none";
    return true;
}