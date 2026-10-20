import api from "../api.js";

const mensaje = document.getElementById("mensaje");
const datosAdministrador = document.getElementById("datosAdministrador");
const botonEliminar = document.getElementById("botonEliminar");

const parametros = new URLSearchParams(window.location.search);
const id = parametros.get("id");

let adminOriginal;

function mostrarError(texto) {
    mensaje.className = "alert alert-danger mb-3";
    mensaje.textContent = texto;
}

function mostrarExito(texto) {
    mensaje.className = "alert alert-success mb-3";
    mensaje.textContent = texto;
}

// Comprobar que el ID sea válido.
if (!id || !/^[1-9]\d*$/.test(id)) {
    mostrarError("El ID del administrador no es válido.");
    datosAdministrador.textContent = "";
    botonEliminar.disabled = true;
} else {
    cargarAdministrador();
}

function cargarAdministrador() {
    try {
        const datosGuardados = sessionStorage.getItem("adminEliminar");

        if (!datosGuardados) {
            mostrarError(
                "No se encontraron los datos del administrador. " +
                "Volvé al listado y seleccioná Eliminar nuevamente."
            );

            datosAdministrador.textContent = "";
            botonEliminar.disabled = true;
            return;
        }

        adminOriginal = JSON.parse(datosGuardados);

        // Comprobar que los datos correspondan al ID de la URL.
        if (String(adminOriginal.iD_Admin) !== id) {
            mostrarError(
                "Los datos seleccionados no corresponden al administrador solicitado."
            );

            datosAdministrador.textContent = "";
            botonEliminar.disabled = true;
            return;
        }

        datosAdministrador.textContent =
            `Administrador: ${adminOriginal.admin_Nom_Ape} \n` +
            `Usuario: ${adminOriginal.admin_Nom_Usuario}`;

    } catch (error) {
        console.error("Error al cargar el administrador:", error);

        mostrarError("No se pudieron recuperar los datos del administrador.");
        datosAdministrador.textContent = "";
        botonEliminar.disabled = true;
    }
}

botonEliminar.addEventListener("click", async function () {
    if (!adminOriginal || String(adminOriginal.iD_Admin) !== id) {
        mostrarError("No se pudo verificar el administrador seleccionado.");
        return;
    }

    const confirmar = window.confirm(
        `¿Querés eliminar al administrador ${adminOriginal.admin_Nom_Ape}?`
    );

    if (!confirmar) {
        return;
    }

    mensaje.className = "alert alert-info mb-3";
    mensaje.textContent = "Eliminando administrador...";
    botonEliminar.disabled = true;

    try {
        const response = await api.delete(`/api/admin/${id}`);

        mostrarExito(
            response.data?.mensaje ??
            "Administrador eliminado correctamente."
        );

        sessionStorage.removeItem("adminEliminar");

        setTimeout(() => {
            window.location.href = "./index.html";
        }, 1200);

    } catch (error) {
        console.error("Error al eliminar administrador:", error);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        let textoError;

        if (status === 400) {
            textoError = detalle ||
                "El ID del administrador no es válido.";
        } else if (status === 404) {
            textoError = detalle ||
                "No se encontró el administrador que intentás eliminar.";
        } else if (status === 409) {
            textoError = detalle ||
                "No se puede eliminar el administrador. Puede tratarse del superadministrador.";
        } else if (status === 500) {
            textoError = detalle ||
                "Ocurrió un error interno al eliminar el administrador.";
        } else {
            textoError =
                "No se pudo establecer comunicación con el servidor.";
        }

        mostrarError(textoError);

    } finally {
        botonEliminar.disabled = false;
    }
});