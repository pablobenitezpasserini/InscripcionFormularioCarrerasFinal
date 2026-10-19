using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Results;

namespace Administradores.BR.Interfaces.Services
{
    public interface IAdministradorService
    {
        Task<Result<IEnumerable<Administrador>>> ListarAsync();

        Task<Result<bool>> CrearAsync(CrearAdminDto admin);

        Task<Result<bool>> EditarAsync(int id, EditarAdminDto admin);

        Task<Result<bool>> EliminarAsync(int id);

        Task<Result<string>> LoguearAsync(string usuario, string contrasena);

        Task<Result<IEnumerable<EstudianteExportacionDto>>> ExportarEstudiantesExcelAsync();
    }
}