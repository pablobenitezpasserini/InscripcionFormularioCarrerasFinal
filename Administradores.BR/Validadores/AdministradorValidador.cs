using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Validators;
using Administradores.Models.Administradores.DTOs;

namespace Administradores.BR.Validadores
{
    public class AdministradorValidador : IAdministradorValidator
    {
        public string? ValidarCrear(CrearAdminDto? admin)
        {
            if (admin is null)
                return "Los datos del administrador son obligatorios.";

            if (string.IsNullOrWhiteSpace(admin.Admin_Nom_Ape))
                return "El nombre y apellido no pueden estar vacíos.";

            if (string.IsNullOrWhiteSpace(admin.Admin_DNI))
                return "El DNI no puede estar vacío.";

            if (string.IsNullOrWhiteSpace(admin.Admin_Tipo_DNI))
                return "El tipo de DNI no puede estar vacío.";

            if (string.IsNullOrWhiteSpace(admin.Admin_Nom_Usuario))
                return "El nombre de usuario no puede estar vacío.";

            if (string.IsNullOrWhiteSpace(admin.Admin_Contra))
                return "La contraseña no puede estar vacía.";

            var nombre = admin.Admin_Nom_Ape.Trim();
            var dni = admin.Admin_DNI.Trim();
            var tipoDni = admin.Admin_Tipo_DNI.Trim();
            var usuario = admin.Admin_Nom_Usuario.Trim();
            var contraseña = admin.Admin_Contra;

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