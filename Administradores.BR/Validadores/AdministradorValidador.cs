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

        /// <summary>
        /// Valida los datos proporcionados para un administrador, verificando que los campos obligatorios
        /// estén completos, que el DNI tenga un formato válido y que los campos respeten las longitudes permitidas.
        /// </summary>
        /// <param name="nombre">Nombre y apellido del administrador.</param>
        /// <param name="dni">Número de documento de identidad del administrador.</param>
        /// <param name="tipoDni">Tipo de documento de identidad del administrador.</param>
        /// <param name="usuario">Nombre de usuario para iniciar sesión.</param>
        /// <param name="contraseña">Contraseña de acceso del administrador.</param>
        /// <returns>
        /// Un mensaje descriptivo si alguno de los datos no cumple con las condiciones establecidas;
        /// de lo contrario, <see langword="null"/>.
        /// </returns>
        private static string? ValidarDatos(string? nombre, string? dni, string? tipoDni, string? usuario, string? contraseña)
        {
            var errorObligatorios = ValidarCamposObligatorios(
                nombre,
                dni,
                tipoDni,
                usuario,
                contraseña);

            if (errorObligatorios is not null)
                return errorObligatorios;

            nombre = nombre!.Trim();
            dni = dni!.Trim();
            tipoDni = tipoDni!.Trim();
            usuario = usuario!.Trim();

            var errorDni = ValidarFormatoDni(dni);

            if (errorDni is not null)
                return errorDni;

            return ValidarLongitudes(
                nombre,
                tipoDni,
                usuario,
                contraseña!);
        }

        /// <summary>
        /// Verifica que todos los campos necesarios para registrar o modificar un administrador
        /// contengan valores y no estén vacíos o compuestos únicamente por espacios en blanco.
        /// </summary>
        /// <param name="nombre">Nombre y apellido del administrador.</param>
        /// <param name="dni">Número de documento de identidad del administrador.</param>
        /// <param name="tipoDni">Tipo de documento de identidad del administrador.</param>
        /// <param name="usuario">Nombre de usuario para iniciar sesión.</param>
        /// <param name="contraseña">Contraseña de acceso del administrador.</param>
        /// <returns>
        /// Un mensaje descriptivo correspondiente al primer campo obligatorio que no cumpla la validación;
        /// de lo contrario, <see langword="null"/>.
        /// </returns>
        private static string? ValidarCamposObligatorios(string? nombre, string? dni, string? tipoDni, string? usuario, string? contraseña)
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

            return null;
        }

        /// <summary>
        /// Valida que el DNI contenga exclusivamente caracteres numéricos y tenga exactamente 8 caracteres.
        /// </summary>
        /// <param name="dni">Número de documento de identidad que se desea validar.</param>
        /// <returns>
        /// Un mensaje descriptivo si el DNI no cumple con el formato establecido;
        /// de lo contrario, <see langword="null"/>.
        /// </returns>
        private static string? ValidarFormatoDni(string dni)
        {
            if (dni.Any(c => c < '0' || c > '9'))
                return "El DNI solo puede contener números.";

            if (dni.Length != 8)
                return "El DNI tiene que contener 8 caracteres.";

            return null;
        }

        /// <summary>
        /// Verifica que la longitud de los datos del administrador no supere los límites establecidos
        /// para cada campo.
        /// </summary>
        /// <param name="nombre">Nombre y apellido del administrador.</param>
        /// <param name="tipoDni">Tipo de documento de identidad del administrador.</param>
        /// <param name="usuario">Nombre de usuario para iniciar sesión.</param>
        /// <param name="contraseña">Contraseña de acceso del administrador.</param>
        /// <returns>
        /// Un mensaje descriptivo si alguno de los campos supera la longitud máxima permitida;
        /// de lo contrario, <see langword="null"/>.
        /// </returns>
        private static string? ValidarLongitudes(string nombre, string tipoDni, string usuario, string contraseña)
        {
            if (nombre.Length > 100)
                return "El nombre y apellido no pueden superar los 100 caracteres.";

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