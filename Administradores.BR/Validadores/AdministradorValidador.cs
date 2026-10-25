using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Validators;
using Administradores.Models.Administradores.DTOs;

namespace Administradores.BR.Validadores
{
    /// <summary>
    /// Implementa las validaciones de los datos de los administradores,
    /// incluyendo campos obligatorios, longitudes máximas y formato del DNI.
    /// </summary>
    public class AdministradorValidador : IAdministradorValidator
    {
        /// <inheritdoc />
        public string? ValidarCrear(CrearAdminDto? admin)
        {
            if (admin is null)
                return "Los datos del administrador son obligatorios.";

            return ValidarDatos(
                admin.Admin_Nom_Ape,
                admin.Admin_DNI,
                admin.Admin_Tipo_DNI,
                admin.Admin_Nom_Usuario,
                admin.Admin_Contra);
        }
        /// <inheritdoc />
        public string? ValidarEditar(EditarAdminDto? admin)
        {
            if (admin is null)
                return "Los datos del administrador son obligatorios.";

            return ValidarDatos(
                admin.Admin_Nom_Ape,
                admin.Admin_DNI,
                admin.Admin_Tipo_DNI,
                admin.Admin_Nom_Usuario,
                admin.Admin_Contra);
        }

        private string? ValidarDatos(
            string? nombre,
            string? dni,
            string? tipoDni,
            string? usuario,
            string? contraseña)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre y apellido son obligatorios.";

            if (string.IsNullOrWhiteSpace(dni))
                return "El DNI es obligatorio.";

            if (string.IsNullOrWhiteSpace(tipoDni))
                return "El tipo de DNI es obligatorio.";

            if (string.IsNullOrWhiteSpace(usuario))
                return "El nombre de usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(contraseña))
                return "La contraseña es obligatoria.";

            nombre = nombre.Trim();
            dni = dni.Trim();
            tipoDni = tipoDni.Trim();
            usuario = usuario.Trim();

            if (dni.Any(c => c < '0' || c > '9'))
                return "El DNI solo puede contener números.";

            if (nombre.Length > 100)
                return "El nombre y apellido no pueden superar los 100 caracteres.";

            if (dni.Length != 8)
                return "El DNI tiene que contener 8 caracteres.";

            if (tipoDni.Length > 40)
                return "El tipo de DNI no puede superar los 40 caracteres.";

            if (usuario.Length > 30)
                return "El nombre de usuario no puede superar los 30 caracteres.";

            if (contraseña.Length > 255)
                return "La contraseña no puede superar los 255 caracteres.";

            return null;
        }
    }
}