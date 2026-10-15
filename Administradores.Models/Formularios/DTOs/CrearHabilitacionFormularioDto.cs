using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Formularios.DTOs
{
    public class CrearHabilitacionFormularioDto
    {
        public int Hab_Form_Año { get; set; }

        public DateTime Hab_Form_Fecha_Inicio { get; set; }

        public DateTime Hab_Form_Fecha_Cierre { get; set; }
    }
}