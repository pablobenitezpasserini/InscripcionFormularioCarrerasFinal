import api from "../api.js";

const mensaje = document.getElementById("mensaje");
const datosFormulario = document.getElementById("datosFormulario");
const accionesError = document.getElementById("accionesError");
const botonEliminar = document.getElementById("botonEliminar");

const anio = document.getElementById("anio");
const fechaInicio = document.getElementById("fechaInicio");
const fechaCierre = document.getElementById("fechaCierre");
const estado = document.getElementById("estado");

function mostrarMensaje(texto, tipo) {
    mensaje.textContent = texto;
    mensaje.className = `alert alert-${tipo} mb-3`;
    mensaje.hidden = false;
}

function formatearFecha(fecha) {
    if (!fecha) return "";

    return new Date(fecha).toLocaleDateString("es-AR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        timeZone: "UTC"
    });
}

const parametros = new URLSearchParams(window.location.search);
const id = parametros.get("id");
const formularioGuardado = sessionStorage.getItem("formularioEliminar");

let formularioOriginal = null;

if (!id || !formularioGuardado) {
    mostrarMensaje(
        "No se encontraron los datos del formulario. Volvé al listado y seleccioná uno para eliminar.",
        "danger"
    );

    accionesError.hidden = false;
} else {
    try {
        formularioOriginal = JSON.parse(formularioGuardado);

        if (String(formularioOriginal.iD_Hab_Form) !== id) {
            formularioOriginal = null;

            mostrarMensaje(
                "El formulario seleccionado no coincide con el ID de la URL.",
                "danger"
            );

            accionesError.hidden = false;
        } else {
            cargarDatosFormulario(formularioOriginal);
        }
    } catch (error) {
        console.error("Error al recuperar el formulario:", error);

        formularioOriginal = null;

        mostrarMensaje(
            "No se pudieron recuperar los datos del formulario.",
            "danger"
        );

        accionesError.hidden = false;
    }
}

function cargarDatosFormulario(formulario) {
    anio.value = formulario.hab_Form_Año ?? "";
    fechaInicio.value = formatearFecha(formulario.hab_Form_Fecha_Inicio);
    fechaCierre.value = formatearFecha(formulario.hab_Form_Fecha_Cierre);
    estado.value = formulario.hab_Form_Estado ?? "";

    datosFormulario.hidden = false;
}

botonEliminar.addEventListener("click", async () => {
    if (!formularioOriginal || !id) {
        mostrarMensaje(
            "No se puede eliminar el formulario porque no se encontraron sus datos.",
            "danger"
        );
        return;
    }

    try {
        botonEliminar.disabled = true;
        botonEliminar.textContent = "Eliminando...";

        await api.delete(`/api/formularios/${encodeURIComponent(id)}`);

        mostrarMensaje("Formulario eliminado correctamente.", "success");

        datosFormulario.hidden = true;
        sessionStorage.removeItem("formularioEliminar");

        window.setTimeout(() => {
            window.location.href = "./index.html";
        }, 1200);

    } catch (error) {
        console.error("Error al eliminar el formulario:", error);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        if (status === 400) {
            mostrarMensaje(
                detalle ?? "No se puede eliminar el formulario porque los datos no son válidos.",
                "danger"
            );
        } else if (status === 404) {
            mostrarMensaje(
                detalle ?? "El formulario que intentás eliminar no existe o ya fue eliminado.",
                "danger"
            );
        } else if (status === 500) {
            mostrarMensaje(
                detalle ?? "Ocurrió un error interno al eliminar el formulario.",
                "danger"
            );
        } else {
            mostrarMensaje(
                "No se pudo conectar con el servidor. Intentá nuevamente.",
                "danger"
            );
        }
    } finally {
        botonEliminar.disabled = false;
        botonEliminar.textContent = "Eliminar formulario";
    }
});