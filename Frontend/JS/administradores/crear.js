import api from "../api.js";

const formulario = document.getElementById("crearAdminForm");
const mensaje = document.getElementById("mensaje");
const botonCrear = document.getElementById("crearAdmin");

formulario.addEventListener("submit", async function (e) {
    e.preventDefault();

    // Obtener los datos del formulario
    const admin = {
        Admin_Nom_Ape: document.getElementById("nombreApellido").value.trim(),
        Admin_DNI: document.getElementById("dni").value.trim(),
        Admin_Tipo_DNI: document.getElementById("tipoDni").value,
        Admin_Nom_Usuario: document.getElementById("nombreUsuario").value.trim(),
        Admin_Contra: document.getElementById("contrasena").value
    };

    // Mostrar estado de carga
    mensaje.className = "alert alert-info text-start";
    mensaje.textContent = "Registrando administrador...";
    botonCrear.disabled = true;

    try {
        const response = await api.post("/api/admin", admin);

        mensaje.className = "alert alert-success text-start";
        mensaje.textContent =
            response.data?.mensaje ??
            "Administrador creado correctamente.";

        // Volver al listado después de mostrar el resultado
        setTimeout(() => {
            window.location.href = "./index.html";
        }, 1200);

    } catch (error) {
        console.error("Error al crear administrador:", error);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        let textoError;

        if (status === 400) {
            textoError = detalle || "Los datos ingresados no son válidos.";
        } else if (status === 409) {
            textoError = detalle ||
                "No se pudo registrar el administrador porque existe un conflicto con los datos ingresados.";
        } else if (status === 500) {
            textoError = detalle ||
                "Ocurrió un error interno al registrar el administrador.";
        } else {
            textoError = detalle ||
                "No se pudo establecer comunicación con el servidor.";
        }

        mensaje.className = "alert alert-danger text-start";
        mensaje.textContent = textoError;

    } finally {
        botonCrear.disabled = false;
    }
});