using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Formularios.Models
{
    /// <summary>
    /// Representa una habilitación de formulario de inscripción del sistema.
    /// </summary>
    public class HabilitacionFormulario
    {
        /// <summary>
        /// Identificador único del formulario.
        /// </summary>
        public int ID_Hab_Form { get; set; }
        /// <summary>
        /// Año correspondiente al período de inscripción.
        /// </summary>
        public int Hab_Form_Año { get; set; }
        /// <summary>
        /// Fecha de la cual se habilita el formulario de inscripción.
        /// </summary>
        public DateTime Hab_Form_Fecha_Inicio { get; set; }
        /// <summary>
        /// Fecha en la que finaliza el período de inscripción.
        /// </summary>
        public DateTime Hab_Form_Fecha_Cierre { get; set; }
        /// <summary>
        /// Estado de la habilitación del formulario, que indica si se encuentra habilitado o deshabilitado.
        /// </summary>
        public string? Hab_Form_Estado { get; set; }
    }
}