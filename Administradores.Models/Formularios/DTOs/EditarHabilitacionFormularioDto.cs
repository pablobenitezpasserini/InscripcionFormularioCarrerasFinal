using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Formularios.DTOs
{
    /// <summary>
    /// Representa los datos necesarios para modificar una habilitación de formulario de inscripción existente.
    /// </summary>
    public class EditarHabilitacionFormularioDto
    {
        /// <summary>
        /// Fecha de la cual se habilita el formulario de inscripción.
        /// </summary>
        public DateTime Hab_Form_Fecha_Inicio { get; set; }
        /// <summary>
        /// Fecha en la que finaliza el período de inscripción.
        /// </summary>
        public DateTime Hab_Form_Fecha_Cierre { get; set; }
        /// <summary>
        /// Estado actual de la habilitación del formulario, que indica si se encuentra habilitado o deshabilitado.
        /// </summary>
        public string Hab_Form_Estado { get; set; } = string.Empty;
    }
}