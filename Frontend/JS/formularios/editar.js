import api from "../api.js";

const formularioEditar = document.getElementById("formularioEditar");
const mensaje = document.getElementById("mensaje");
const botonGuardar = document.getElementById("botonGuardar");

const anio = document.getElementById("anio");
const fechaInicio = document.getElementById("fechaInicio");
const fechaCierre = document.getElementById("fechaCierre");
const estado = document.getElementById("estado");

function mostrarMensaje(texto, tipo) {
    mensaje.textContent = texto;
    mensaje.className = `alert alert-${tipo} mb-3`;
    mensaje.hidden = false;
}

// Obtener el ID desde la URL.
const parametros = new URLSearchParams(window.location.search);
const id = parametros.get("id");

// Recuperar el formulario guardado en sessionStorage.
const formularioGuardado = sessionStorage.getItem("formularioEditar");

let formularioOriginal = null;

if (!id || !formularioGuardado) {
    mostrarMensaje(
        "No se encontraron los datos del formulario. Volvé al listado y seleccioná uno para editar.",
        "danger"
    );
} else {
    try {
        formularioOriginal = JSON.parse(formularioGuardado);

        if (String(formularioOriginal.iD_Hab_Form) !== id) {
            formularioOriginal = null;

            mostrarMensaje(
                "El formulario seleccionado no coincide con el ID de la URL.",
                "danger"
            );
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
    }
}

function cargarDatosFormulario(formulario) {
    anio.value = formulario.hab_Form_Año;

    // El input date necesita el formato YYYY-MM-DD.
    fechaInicio.value =
        formulario.hab_Form_Fecha_Inicio?.substring(0, 10) ?? "";

    fechaCierre.value =
        formulario.hab_Form_Fecha_Cierre?.substring(0, 10) ?? "";

    estado.value = formulario.hab_Form_Estado;

    // Verificar que los datos sean válidos.
    if (
        !fechaInicio.value ||
        !fechaCierre.value ||
        !["HABILITADO", "DESHABILITADO"].includes(estado.value)
    ) {
        mostrarMensaje(
            "Los datos del formulario están incompletos o contienen un estado inválido.",
            "danger"
        );
        return;
    }

    mensaje.textContent = "";
    mensaje.hidden = true;
    formularioEditar.hidden = false;
}

formularioEditar.addEventListener("submit", async (event) => {
    event.preventDefault();

    if (!formularioOriginal) {
        mostrarMensaje(
            "No se puede editar el formulario porque no se encontraron sus datos.",
            "danger"
        );
        return;
    }

    mensaje.textContent = "";
    mensaje.hidden = true;

    if (fechaInicio.value > fechaCierre.value) {
        mostrarMensaje(
            "La fecha de inicio no puede ser posterior a la fecha de cierre.",
            "danger"
        );
        fechaInicio.focus();
        return;
    }

    const datosFormulario = {
        hab_Form_Fecha_Inicio: fechaInicio.value,
        hab_Form_Fecha_Cierre: fechaCierre.value,
        hab_Form_Estado: estado.value
    };

    try {
        botonGuardar.disabled = true;
        botonGuardar.textContent = "Guardando...";

        await api.put(
            `/api/formularios/${encodeURIComponent(id)}`,
            datosFormulario
        );

        mostrarMensaje(
            "Formulario actualizado correctamente.",
            "success"
        );

        sessionStorage.removeItem("formularioEditar");

        window.setTimeout(() => {
            window.location.href = "./index.html";
        }, 1200);

    } catch (error) {
        console.error("Error al editar el formulario:", error);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        if (status === 400) {
            mostrarMensaje(
                detalle ?? "Los datos ingresados no son válidos.",
                "danger"
            );
        } else if (status === 404) {
            mostrarMensaje(
                detalle ?? "No se encontró el formulario que querés editar.",
                "danger"
            );
        } else if (status === 409) {
            mostrarMensaje(
                detalle ?? "No se pudo actualizar el formulario por un conflicto.",
                "danger"
            );
        } else if (status === 500) {
            mostrarMensaje(
                detalle ?? "Ocurrió un error interno al actualizar el formulario.",
                "danger"
            );
        } else {
            mostrarMensaje(
                "No se pudo conectar con el servidor. Intentá nuevamente.",
                "danger"
            );
        }

    } finally {
        botonGuardar.disabled = false;
        botonGuardar.textContent = "Guardar cambios";
    }
});