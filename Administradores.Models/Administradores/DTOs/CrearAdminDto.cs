using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.DTOs
{
    public class CrearAdminDto
    {
        public string? Admin_Nom_Ape { get; set; }
        public string? Admin_DNI { get; set; }
        public string? Admin_Tipo_DNI { get; set; }
        public string? Admin_Nom_Usuario { get; set; }
        public string? Admin_Contra { get; set; }
    }
}