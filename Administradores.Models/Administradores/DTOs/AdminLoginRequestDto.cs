using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Administradores.DTOs
{
    public class AdminLoginRequestDto
    {
        public string? Usuario { get; set; }
        public string? Contrasena { get; set; }
    }
}