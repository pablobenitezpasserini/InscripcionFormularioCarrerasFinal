using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.DTOs
{
    /// <summary>
    /// Representa los datos necesarios para registrar un nuevo administrador en el sistema.
    /// </summary>
    public class CrearAdminDto
    {
        /// <summary>
        /// Nombre y apellido del administrador.
        /// </summary>
        public string? Admin_Nom_Ape { get; set; }
        /// <summary>
        /// Número de documento de identidad del administrador.
        /// </summary>
        public string? Admin_DNI { get; set; }
        /// <summary>
        /// Tipo de documento de identidad del administrador.
        /// </summary>
        public string? Admin_Tipo_DNI { get; set; }
        /// <summary>
        /// Nombre de usuario utilizado para iniciar sesión.
        /// </summary>
        public string? Admin_Nom_Usuario { get; set; }
        /// <summary>
        /// Contraseña del administrador.
        /// </summary>
        public string? Admin_Contra { get; set; }
    }
}