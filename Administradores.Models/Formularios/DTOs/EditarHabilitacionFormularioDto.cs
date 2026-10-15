using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Formularios.DTOs
{
    public class EditarHabilitacionFormularioDto
    {
        public DateTime Hab_Form_Fecha_Inicio { get; set; }

        public DateTime Hab_Form_Fecha_Cierre { get; set; }

        public string Hab_Form_Estado { get; set; } = string.Empty;
    }
}