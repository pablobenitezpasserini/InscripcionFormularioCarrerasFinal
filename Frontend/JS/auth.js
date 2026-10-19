const AUTH_KEY = "logueado";

export function estaLogueado() {
    return localStorage.getItem(AUTH_KEY) === "true";
}

export function protegerPagina() {
    if (!estaLogueado()) {
        window.location.replace(
            new URL("../login/", import.meta.url).href
        );
    }
}

export function cerrarSesion() {
    localStorage.removeItem(AUTH_KEY);

    window.location.replace(
        new URL("../login/", import.meta.url).href
    );
}