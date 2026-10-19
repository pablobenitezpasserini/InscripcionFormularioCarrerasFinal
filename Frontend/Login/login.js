import api from "../JS/api.js";

const loginForm = document.getElementById("loginForm");

// Login
loginForm.addEventListener("submit", async function (e) {
    e.preventDefault();

    const usuario = document.getElementById("loginUser").value;
    const contrasena = document.getElementById("loginPassword").value;

    try {
        const response = await api.post("/api/admin/login", {
            usuario: usuario,
            contrasena: contrasena
        });

        console.log("Respuesta del servidor:", response.data);

        if (response.data.mensaje === "Logueo exitoso.") {
            alert("Logueo correcto");

            localStorage.setItem("logueado", "true");

            window.location.href = "../menu/";
        } else {
            alert(response.data.mensaje);
        }

    } catch (error) {
        console.error("Error al loguear:", error);

        if (error.response?.data?.mensaje) {
            alert(error.response.data.mensaje);
        } else {
            alert("Error al comunicarse con el servidor.");
        }
    }
});