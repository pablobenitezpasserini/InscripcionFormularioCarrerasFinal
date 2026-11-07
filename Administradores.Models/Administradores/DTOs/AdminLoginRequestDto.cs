using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.DTOs
{
    /// <summary>
    /// Representa los datos necesarios para realizar la autenticación de un administrador en el sistema.
    /// </summary>
    public class AdminLoginRequestDto
    {
        /// <summary>
        /// Nombre de usuario del administrador.
        /// </summary>
        public string? Usuario { get; set; }
        /// <summary>
        /// Contraseña del administrador.
        /// </summary>
        public string? Contrasena { get; set; }
    }
}