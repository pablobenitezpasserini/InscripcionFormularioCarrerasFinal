using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Formularios.DTOs
{
    /// <summary>
    /// Representa los datos necesarios para crear una nueva habilitación de formulario de inscripción.
    /// </summary>
    public class CrearHabilitacionFormularioDto
    {
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
    }
}