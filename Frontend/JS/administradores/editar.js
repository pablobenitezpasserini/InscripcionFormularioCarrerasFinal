import api from "../api.js";

const formulario = document.getElementById("editarAdminForm");
const mensaje = document.getElementById("mensaje");
const botonGuardar = document.getElementById("botonGuardar");

const parametros = new URLSearchParams(window.location.search);
const id = parametros.get("id");

let adminOriginal;

// Verificar que exista un ID válido
if (!id || !/^[1-9]\d*$/.test(id)) {
    mostrarError("El ID del administrador no es válido.");
    formulario.querySelectorAll("input, select, button").forEach(
        elemento => elemento.disabled = true
    );
} else {
    cargarAdministrador();
}

function mostrarError(texto) {
    mensaje.className = "alert alert-danger text-start";
    mensaje.textContent = texto;
}

function mostrarExito(texto) {
    mensaje.className = "alert alert-success text-start";
    mensaje.textContent = texto;
}

function cargarAdministrador() {
    try {
        const datosGuardados = sessionStorage.getItem("adminEditar");

        if (!datosGuardados) {
            mostrarError(
                "No se encontraron los datos del administrador. Volvé al listado y seleccioná Editar nuevamente."
            );
            formulario.querySelectorAll("input, select, button").forEach(
                elemento => elemento.disabled = true
            );
            return;
        }

        adminOriginal = JSON.parse(datosGuardados);

        // Comprobar que el objeto corresponda al ID de la URL
        if (String(adminOriginal.iD_Admin) !== id) {
            mostrarError(
                "Los datos seleccionados no corresponden al administrador solicitado."
            );
            formulario.querySelectorAll("input, select, button").forEach(
                elemento => elemento.disabled = true
            );
            return;
        }

        // Completar los campos del formulario
        document.getElementById("nombreApellido").value =
            adminOriginal.admin_Nom_Ape ?? "";

        document.getElementById("tipoDni").value =
            adminOriginal.admin_Tipo_DNI ?? "";

        document.getElementById("dni").value =
            adminOriginal.admin_DNI ?? "";

        document.getElementById("nombreUsuario").value =
            adminOriginal.admin_Nom_Usuario ?? "";

        // La implementación actual del service exige una contraseña.
        document.getElementById("contrasena").value =
            adminOriginal.admin_Contra ?? "";

        // No mostramos ni guardamos la contraseña en un mensaje.
        if (!adminOriginal.admin_Contra) {
            mostrarError(
                "El administrador no tiene una contraseña disponible para completar la edición."
            );
        }

    } catch (error) {
        console.error("Error al cargar el administrador:", error);
        mostrarError("No se pudieron recuperar los datos del administrador.");
    }
}

formulario.addEventListener("submit", async function (e) {
    e.preventDefault();

    const admin = {
        Admin_Nom_Ape:
            document.getElementById("nombreApellido").value.trim(),

        Admin_DNI:
            document.getElementById("dni").value.trim(),

        Admin_Tipo_DNI:
            document.getElementById("tipoDni").value,

        Admin_Nom_Usuario:
            document.getElementById("nombreUsuario").value.trim(),

        Admin_Contra:
            document.getElementById("contrasena").value
    };

    mensaje.className = "alert alert-info text-start";
    mensaje.textContent = "Guardando cambios...";
    botonGuardar.disabled = true;

    try {
        const response = await api.put(`/api/admin/${id}`, admin);

        mostrarExito(
            response.data?.mensaje ??
            "Administrador modificado correctamente."
        );

        // Eliminar los datos temporales de la edición
        sessionStorage.removeItem("adminEditar");

        setTimeout(() => {
            window.location.href = "./index.html";
        }, 1200);

    } catch (error) {
        console.error("Error al editar administrador:", error);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        let textoError;

        if (status === 400) {
            textoError = detalle ||
                "Los datos ingresados no son válidos.";
        } else if (status === 404) {
            textoError = detalle ||
                "No se encontró el administrador que intentás modificar.";
        } else if (status === 409) {
            textoError = detalle ||
                "Existe un conflicto con los datos ingresados.";
        } else if (status === 500) {
            textoError = detalle ||
                "Ocurrió un error interno al modificar el administrador.";
        } else {
            textoError = detalle ||
                "No se pudo establecer comunicación con el servidor.";
        }

        mostrarError(textoError);

    } finally {
        botonGuardar.disabled = false;
    }
});