using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Administradores.Models;

namespace Administradores.BR.Interfaces.Repositories
{
    public interface IAdministradorRepository
    {
        Task<IEnumerable<Administrador>> ListarAsync();

        Task CrearAsync(Administrador admin);

        Task EditarAsync(Administrador admin);

        Task EliminarAsync(int id);

        Task<string> LoguearAsync(string usuario, string contrasena);

        Task<bool> ExisteDniAsync(string dni, string tipoDni);

        Task<bool> ExisteDniAsync(string dni, string tipoDni, int idExcluir);

        Task<bool> ExisteUsuarioAsync(string nombreUsuario);

        Task<bool> ExistePorIdAsync(int id);

        Task<bool> ExisteUsuarioAsync(string nombreUsuario, int idExcluir);

        Task<bool> EsSuperAdminAsync(int id);
    }
}