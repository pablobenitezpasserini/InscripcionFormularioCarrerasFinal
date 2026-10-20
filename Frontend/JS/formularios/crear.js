import api from "../api.js";

const formularioCrear = document.getElementById("formularioCrear");
const mensaje = document.getElementById("mensaje");
const botonGuardar = document.getElementById("botonGuardar");

const anio = document.getElementById("anio");
const fechaInicio = document.getElementById("fechaInicio");
const fechaCierre = document.getElementById("fechaCierre");

function mostrarMensaje(texto, tipo) {
    mensaje.textContent = texto;
    mensaje.className = `alert alert-${tipo} mb-3`;
    mensaje.hidden = false;
}

formularioCrear.addEventListener("submit", async (event) => {
    event.preventDefault();

    // Ocultar el mensaje anterior.
    mensaje.textContent = "";
    mensaje.hidden = true;

    const añoIngresado = Number(anio.value);

    if (!Number.isInteger(añoIngresado) || añoIngresado < 1) {
        mostrarMensaje("Ingresá un año válido.", "danger");
        anio.focus();
        return;
    }

    if (fechaInicio.value > fechaCierre.value) {
        mostrarMensaje(
            "La fecha de inicio no puede ser posterior a la fecha de cierre.",
            "danger"
        );
        fechaInicio.focus();
        return;
    }

    const datosFormulario = {
        hab_Form_Año: añoIngresado,
        hab_Form_Fecha_Inicio: fechaInicio.value,
        hab_Form_Fecha_Cierre: fechaCierre.value
    };

    try {
        botonGuardar.disabled = true;
        botonGuardar.textContent = "Guardando...";

        await api.post("/api/formularios", datosFormulario);

        mostrarMensaje("Formulario creado correctamente.", "success");

        formularioCrear.reset();

        window.setTimeout(() => {
            window.location.href = "./index.html";
        }, 1200);

    } catch (error) {
        console.error("Error al crear el formulario:", error);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        if (status === 400) {
            mostrarMensaje(
                detalle ?? "Los datos ingresados no son válidos.",
                "danger"
            );
        } else if (status === 409) {
            mostrarMensaje(
                detalle ?? "No se pudo crear el formulario por un conflicto.",
                "danger"
            );
        } else if (status === 500) {
            mostrarMensaje(
                detalle ?? "Ocurrió un error interno al crear el formulario.",
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
        botonGuardar.textContent = "Guardar formulario";
    }
});