import api from "../JS/api.js";

const tablaEstudiantes = document.getElementById("tablaEstudiantes");
const mensaje = document.getElementById("mensaje");
const busqueda = document.getElementById("busqueda");
const cantidadRegistros = document.getElementById("cantidadRegistros");
const botonExportar = document.getElementById("botonExportar");

let estudiantes = [];

const columnas = [
    { clave: "fechaInscripcion", titulo: "Marca temporal" },
    { clave: "correoElectronico", titulo: "Correo electrónico" },
    { clave: "carreraInscripta", titulo: "Carrera inscripta" },
    { clave: "nombreApellido", titulo: "Nombre y apellido" },
    { clave: "dni", titulo: "DNI" },
    { clave: "telefono", titulo: "Teléfono" },
    { clave: "fechaNacimiento", titulo: "Fecha de nacimiento" },
    { clave: "edadActual", titulo: "Edad al 30/06" },
    { clave: "direccion", titulo: "Dirección" },
    { clave: "posee", titulo: "Posee" },
    { clave: "tituloSecundario", titulo: "Título secundario" },
    { clave: "añoEgreso", titulo: "Año de egreso" }
];

function mostrarMensaje(texto, tipo) {
    mensaje.textContent = texto;
    mensaje.className = `alert alert-${tipo} mb-3`;
    mensaje.hidden = false;
}

function ocultarMensaje() {
    mensaje.textContent = "";
    mensaje.className = "mb-3";
    mensaje.hidden = true;
}

function obtenerValor(valor) {
    if (valor === null || valor === undefined) {
        return "";
    }

    return String(valor);
}

function formatearFecha(valor) {
    if (!valor) {
        return "";
    }

    // Me quedo solamente con la parte de la fecha
    const fecha = String(valor).split("T")[0];

    // Separo año, mes y día mediante desestructuracion de array
    const [año, mes, dia] = fecha.split("-");

    // Devuelvo la fecha con formato día/mes/año
    return `${dia}/${mes}/${año}`;
}

function renderizarTabla(lista) {
    // Limpio las filas existentes para renderizar de nuevo
    tablaEstudiantes.replaceChildren();

    // Creo un lugar temporal para almacenar las filas
    // sin tener que agregarlas una por una al DOM
    const fragmento = document.createDocumentFragment();

    lista.forEach(estudiante => {
        const fila = document.createElement("tr");

        columnas.forEach(columna => {
            const celda = document.createElement("td");

            const valor = obtenerValor(estudiante[columna.clave]);

            // Formateo solamente las columnas que contienen fechas
            if (columna.clave === "fechaInscripcion" || columna.clave === "fechaNacimiento" || columna.clave === "añoEgreso") {
                celda.textContent = formatearFecha(valor);
            } else {
                celda.textContent = valor;
            }

            // Si tiene texto, muestra una descripción al pasar el mouse
            if (celda.textContent) {
                celda.title = celda.textContent;
            }

            fila.appendChild(celda);
        });

        fragmento.appendChild(fila);
    });

    tablaEstudiantes.appendChild(fragmento);

    cantidadRegistros.textContent =
        `Registros: ${lista.length}`;
}

async function cargarEstudiantes() {
    tablaEstudiantes.replaceChildren();
    cantidadRegistros.textContent = "Cargando...";

    try {
        const response = await api.get("/api/admin/exportar-estudiantes");

        if (!Array.isArray(response.data)) {
            throw new Error("La API no devolvió una lista válida.");
        }

        estudiantes = response.data;

        renderizarTabla(estudiantes);

        if (estudiantes.length === 0) {
            mostrarMensaje(
                "No hay inscripciones registradas para mostrar.",
                "info"
            );
        } else {
            ocultarMensaje();
        }
    } catch (error) {
        console.error("Error al obtener estudiantes:", error);

        estudiantes = [];
        renderizarTabla([]);

        const status = error.response?.status;
        const detalle = error.response?.data?.detail;

        if (status === 404) {
            mostrarMensaje(
                detalle ?? "No se encontraron inscripciones.",
                "warning"
            );
        } else if (status === 500) {
            mostrarMensaje(
                "Ocurrió un error interno al obtener las inscripciones.",
                "danger"
            );
        } else {
            mostrarMensaje(
                "No se pudieron cargar las inscripciones. Verificá la conexión con el servidor.",
                "danger"
            );
        }
    }
}

busqueda.addEventListener("input", () => {
    const texto = busqueda.value.trim().toLocaleLowerCase("es");
    // filter me devuelve un array nuevo con los datos que coinciden
    // utilizo columnas para obtener los atributos del estudiante
    // y comparo el valor del atributo con el texto de la busqueda
    // esto sucede en el include
    const filtrados = estudiantes.filter(estudiante =>
        columnas.some(columna =>
            obtenerValor(estudiante[columna.clave])
                .toLocaleLowerCase("es")
                .includes(texto)
        )
    );

    renderizarTabla(filtrados);

    if (estudiantes.length > 0 && filtrados.length === 0) {
        mostrarMensaje(
            "No se encontraron inscripciones que coincidan con la búsqueda.",
            "info"
        );
    } else {
        ocultarMensaje();
    }
});

botonExportar.addEventListener("click", async () => {
    if (estudiantes.length === 0) {
        mostrarMensaje(
            "No hay inscripciones para exportar.",
            "warning"
        );
        return;
    }

    if (typeof ExcelJS === "undefined") {
        mostrarMensaje(
            "No se pudo cargar ExcelJS. Revisá la conexión e intentá nuevamente.",
            "danger"
        );
        return;
    }

    try {
        botonExportar.disabled = true;
        botonExportar.textContent = "Generando Excel...";

        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet("Inscripciones");

        worksheet.columns = [
            { header: "Marca temporal", key: "fechaInscripcion", width: 30 },
            { header: "Correo electrónico", key: "correoElectronico", width: 30 },
            { header: "Carrera inscripta", key: "carreraInscripta", width: 35 },
            { header: "Nombre y apellido", key: "nombreApellido", width: 30 },
            { header: "DNI", key: "dni", width: 15 },
            { header: "Teléfono", key: "telefono", width: 20 },
            { header: "Fecha de nacimiento", key: "fechaNacimiento", width: 20 },
            { header: "Edad al 30/06", key: "edadActual", width: 16 },
            { header: "Dirección", key: "direccion", width: 35 },
            { header: "Posee", key: "posee", width: 25 },
            { header: "Título secundario", key: "tituloSecundario", width: 30 },
            { header: "Año de egreso", key: "añoEgreso", width: 18 }
        ];

        // Agregar todas las inscripciones cargadas desde la API.
        estudiantes.forEach(estudiante => {
            worksheet.addRow(estudiante);
        });

        // Dar formato al encabezado.
        const encabezado = worksheet.getRow(1);

        encabezado.font = {
            bold: true,
            color: { argb: "FFFFFFFF" }
        };

        encabezado.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: { argb: "FF1A73E8" }
        };

        encabezado.alignment = {
            vertical: "middle",
            wrapText: true
        };

        encabezado.height = 30;

        // Mantener los encabezados visibles en Excel.
        worksheet.views = [
            { state: "frozen", ySplit: 1 }
        ];

        // Generar el archivo.
        const buffer = await workbook.xlsx.writeBuffer();

        const blob = new Blob(
            [buffer],
            {
                type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            }
        );

        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");

        link.href = url;
        link.download = "Inscripciones.xlsx";

        document.body.appendChild(link);
        link.click();
        link.remove();

        URL.revokeObjectURL(url);

        mostrarMensaje(
            "El archivo Excel se generó correctamente.",
            "success"
        );

    } catch (error) {
        console.error("Error al exportar:", error);

        mostrarMensaje(
            "No se pudo generar el archivo Excel. Intentá nuevamente.",
            "danger"
        );
    } finally {
        botonExportar.disabled = false;
        botonExportar.innerHTML =
            '<i class="bi bi-file-earmark-excel"></i> Exportar a Excel';
    }
});

cargarEstudiantes();