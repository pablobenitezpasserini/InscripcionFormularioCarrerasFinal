using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.DTOs
{
    /// <summary>
    /// Representa los datos de un estudiante que se incluyen en la exportación de inscripciones.
    /// </summary>
    public class EstudianteExportacionDto
    {
        /// <summary>
        /// Fecha en la que se realizó la inscripción.
        /// </summary>
        public DateTime FechaInscripcion { get; set; }
        /// <summary>
        /// Correo electrónico del estudiante.
        /// </summary>
        public string? CorreoElectronico { get; set; }
        /// <summary>
        /// Carrera en la que se inscribió el estudiante.
        /// </summary>
        public string? CarreraInscripta { get; set; }
        /// <summary>
        /// Nombre y apellido del estudiante.
        /// </summary>
        public string? NombreApellido { get; set; }
        /// <summary>
        /// Número de documento de identidad del estudiante.
        /// </summary>
        public int DNI { get; set; }
        /// <summary>
        /// Número de teléfono del estudiante.
        /// </summary>
        public string? Telefono { get; set; }
        /// <summary>
        /// Fecha de nacimiento del estudiante.
        /// </summary>
        public DateTime FechaNacimiento { get; set; }
        /// <summary>
        /// Edad actual del estudiante, calculada al 30 de junio del año en curso.
        /// </summary>
        public int EdadActual { get; set; }
        /// <summary>
        /// Dirección de residencia del estudiante.
        /// </summary>
        public string? Direccion { get; set; }
        /// <summary>
        /// Información sobre si el estudiante posee titulo o no.
        /// </summary>
        public string? Posee { get; set; }
        /// <summary>
        /// Información sobre el título secundario del estudiante.
        /// </summary>
        public string? TituloSecundario { get; set; }
        /// <summary>
        /// Año en el que el estudiante egresó del nivel secundario.
        /// </summary>
        public DateTime AñoEgreso { get; set; }
    }
}