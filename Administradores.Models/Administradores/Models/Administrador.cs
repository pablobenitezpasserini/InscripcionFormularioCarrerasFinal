using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.Models
{
    public class Administrador
    {
        public int ID_Admin { get; set; }
        public string Admin_Nom_Ape { get; set; }
        public string Admin_DNI { get; set; }
        public string Admin_Tipo_DNI { get; set; }
        public string Admin_Nom_Usuario { get; set; }
        public string Admin_Contra { get; set; }
    }
}