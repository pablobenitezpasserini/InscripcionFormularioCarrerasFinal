using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.DTOs
{
    public class EstudianteExportacionDto
    {
        public DateTime FechaInscripcion { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? CarreraInscripta { get; set; }
        public string? NombreApellido { get; set; }
        public int DNI { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int EdadActual { get; set; } // edad calculada al 30/06/añoActual
        public string? Direccion { get; set; }
        public string? Posee { get; set; }
        public string? TituloSecundario { get; set; }
        public DateTime AñoEgreso { get; set; }
    }
}