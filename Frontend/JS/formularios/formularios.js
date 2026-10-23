import api from "../api.js";

const tablaFormularios = document.getElementById("tablaFormularios");
const mensaje = document.getElementById("mensaje");
const botonAgregar = document.getElementById("botonAgregar");

async function cargarFormularios() {
    try {
        mensaje.textContent = "Cargando formularios...";
        botonAgregar.disabled = true;

        const response = await api.get("/api/formularios");
        const formularios = response.data;

        tablaFormularios.replaceChildren();

        if (!Array.isArray(formularios)) {
            throw new Error("La API no devolvió una lista válida.");
        }

        // Regla de negocio: como máximo puede haber dos formularios.
        botonAgregar.disabled = formularios.length >= 2;

        if (formularios.length === 0) {
            mensaje.textContent = "No hay formularios registrados.";
            return;
        }

        mensaje.textContent = "";

        formularios.forEach(formulario => {
            const fila = document.createElement("tr");

            // Guardamos el ID sin mostrarlo como columna.
            fila.dataset.id = formulario.iD_Hab_Form;

            const año = document.createElement("td");
            año.textContent = formulario.hab_Form_Año;

            const fechaInicio = document.createElement("td");
            fechaInicio.textContent = formatearFecha(
                formulario.hab_Form_Fecha_Inicio
            );

            const fechaCierre = document.createElement("td");
            fechaCierre.textContent = formatearFecha(
                formulario.hab_Form_Fecha_Cierre
            );

            const estado = document.createElement("td");
            estado.textContent = formulario.hab_Form_Estado ?? "Sin estado";

            const acciones = document.createElement("td");
            acciones.className = "text-end";

            const botonEditar = document.createElement("button");
            botonEditar.type = "button";
            botonEditar.className = "btn btn-primary btn-sm me-2";
            botonEditar.textContent = "Editar";

            botonEditar.addEventListener("click", () => {
                sessionStorage.setItem(
                    "formularioEditar",
                    JSON.stringify(formulario)
                );

                window.location.href =
                    `./editar.html?id=${encodeURIComponent(formulario.iD_Hab_Form)}`;
            });

            const botonEliminar = document.createElement("button");
            botonEliminar.type = "button";
            botonEliminar.className = "btn btn-danger btn-sm";
            botonEliminar.textContent = "Eliminar";

            botonEliminar.addEventListener("click", () => {
                sessionStorage.setItem(
                    "formularioEliminar",
                    JSON.stringify(formulario)
                );

                window.location.href =
                    `./eliminar.html?id=${encodeURIComponent(formulario.iD_Hab_Form)}`;
            });

            acciones.append(botonEditar, botonEliminar);

            fila.append(
                año,
                fechaInicio,
                fechaCierre,
                estado,
                acciones
            );

            tablaFormularios.appendChild(fila);
        });

    } catch (error) {
        console.error("Error al cargar formularios:", error);

        tablaFormularios.replaceChildren();
        botonAgregar.disabled = true;

        mensaje.textContent =
            error.response?.data?.detail ??
            "No se pudo cargar el listado de formularios.";
    } 
}

function formatearFecha(fecha) {
    if (!fecha) {
        return "—";
    }

    const fechaValida = new Date(fecha);

    if (Number.isNaN(fechaValida.getTime())) {
        return "—";
    }

    return fechaValida.toLocaleDateString("es-AR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        timeZone: "UTC"
    });
}

botonAgregar.addEventListener("click", () => {
    window.location.href = "./crear.html";
});

cargarFormularios();