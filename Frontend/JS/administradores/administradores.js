import api from "../api.js";

const tablaAdministradores = document.getElementById("tablaAdministradores");
const mensaje = document.getElementById("mensaje");

async function cargarAdministradores() {
    try {
        mensaje.textContent = "Cargando administradores...";

        const response = await api.get("/api/admin");
        const administradores = response.data;

        tablaAdministradores.replaceChildren();

        if (!administradores || administradores.length === 0) {
            mensaje.textContent = "No hay administradores registrados.";
            return;
        }

        mensaje.textContent = "";

        administradores.forEach(admin => {
            const fila = document.createElement("tr");

            // ID oculto: no se muestra como columna.
            fila.dataset.id = admin.iD_Admin;

            const nombre = document.createElement("td");
            nombre.textContent = admin.admin_Nom_Ape;

            const tipoDni = document.createElement("td");
            tipoDni.textContent = admin.admin_Tipo_DNI;

            const dni = document.createElement("td");
            dni.textContent = admin.admin_DNI;

            const usuario = document.createElement("td");
            usuario.textContent = admin.admin_Nom_Usuario;

            const acciones = document.createElement("td");
            acciones.className = "text-end";

            const botonEditar = document.createElement("button");
            botonEditar.type = "button";
            botonEditar.className = "btn btn-primary btn-sm me-2";
            botonEditar.textContent = "Editar";

            botonEditar.addEventListener("click", () => {
                sessionStorage.setItem(
                    "adminEditar",
                    JSON.stringify(admin)
                );

                window.location.href =
                    `./editar.html?id=${encodeURIComponent(admin.iD_Admin)}`;
            });

            const botonEliminar = document.createElement("button");
            botonEliminar.type = "button";
            botonEliminar.className = "btn btn-danger btn-sm";
            botonEliminar.textContent = "Eliminar";

            botonEliminar.addEventListener("click", () => {
                sessionStorage.setItem(
                    "adminEliminar",
                    JSON.stringify(admin)
                );

                window.location.href =
                    `./eliminar.html?id=${encodeURIComponent(admin.iD_Admin)}`;
            });

            acciones.append(botonEditar, botonEliminar);
            fila.append(nombre, tipoDni, dni, usuario, acciones);

            tablaAdministradores.appendChild(fila);
        });
    } catch (error) {
        console.error("Error al cargar administradores:", error);

        tablaAdministradores.replaceChildren();
        mensaje.textContent =
            error.response?.data?.detail ??
            "No se pudo cargar el listado de administradores.";
    }
}

cargarAdministradores();