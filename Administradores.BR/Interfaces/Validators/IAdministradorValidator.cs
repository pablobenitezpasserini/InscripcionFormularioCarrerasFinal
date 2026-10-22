using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Administradores.DTOs;

namespace Administradores.BR.Interfaces.Validators
{
    public interface IAdministradorValidator
    {
        public string? ValidarCrear(CrearAdminDto? admin);
    }
}