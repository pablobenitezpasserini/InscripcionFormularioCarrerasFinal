using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Formularios.DTOs;
using Administradores.Models.Formularios.Models;
using Administradores.Models.Results;

namespace Administradores.BR.Interfaces.Services
{
    public interface IHabilitacionFormularioService
    {
        Task<Result<IEnumerable<HabilitacionFormulario>>> GetAllAsync();
        Task<Result<bool>> CrearAsync(CrearHabilitacionFormularioDto data);
        Task<Result<bool>> EditarAsync(int id, EditarHabilitacionFormularioDto data);
        Task<Result<bool>> EliminarAsync(int id);
        Task<Result<HabilitacionFormulario>> EstaDisponibleAsync();
    }
}