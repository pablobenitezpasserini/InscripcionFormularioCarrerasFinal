using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.Models
{
    /// <summary>
    /// Representa la entidad de un administrador registrado en el sistema.
    /// </summary>
    public class Administrador
    {
        /// <summary>
        /// Identificador único del administrador.
        /// </summary>
        public int ID_Admin { get; set; }
        /// <summary>
        /// Nombre y apellido del administrador.
        /// </summary>
        public string Admin_Nom_Ape { get; set; } = string.Empty;
        /// <summary>
        /// Número de documento de identidad del administrador.
        /// </summary>
        public string Admin_DNI { get; set; } = string.Empty;
        /// <summary>
        /// Tipo de documento de identidad del administrador.
        /// </summary>
        public string Admin_Tipo_DNI { get; set; } = string.Empty;
        /// <summary>
        /// Nombre de usuario utilizado para iniciar sesión.
        /// </summary>
        public string Admin_Nom_Usuario { get; set; } = string.Empty;
        /// <summary>
        /// Contraseña asociada a la cuenta del administrador.
        /// </summary>
        public string Admin_Contra { get; set; } = string.Empty;
    }
}