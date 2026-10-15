using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Formularios.Models
{
    public class HabilitacionFormulario
    {
        public int ID_Hab_Form { get; set; }

        public int Hab_Form_Año { get; set; }

        public DateTime Hab_Form_Fecha_Inicio { get; set; }

        public DateTime Hab_Form_Fecha_Cierre { get; set; }

        public string? Hab_Form_Estado { get; set; }
    }
}