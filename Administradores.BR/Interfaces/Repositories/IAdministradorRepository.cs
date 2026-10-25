using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;

namespace Administradores.BR.Interfaces.Repositories
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión de administradores
    /// y la consulta de estudiantes para exportación.
    /// </summary>
    public interface IAdministradorRepository
    {
        /// <summary>
        /// Obtiene la lista de administradores registrados en el sistema.
        /// </summary>
        /// <returns>Una colección de administradores registrados.</returns>
        Task<IEnumerable<Administrador>> ListarAsync();

        /// <summary>
        /// Registra un nuevo administrador en el sistema.
        /// </summary>
        /// <param name="admin">Administrador que se desea registrar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task CrearAsync(Administrador admin);

        /// <summary>
        /// Actualiza los datos de un administrador existente.
        /// </summary>
        /// <param name="admin">Administrador con los datos que se desean actualizar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EditarAsync(Administrador admin);

        /// <summary>
        /// Elimina un administrador del sistema mediante su identificador.
        /// </summary>
        /// <param name="id">Identificador del administrador que se desea eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EliminarAsync(int id);

        /// <summary>
        /// Verifica las credenciales de un usuario para iniciar sesión.
        /// </summary>
        /// <param name="usuario">Nombre de usuario que se desea autenticar.</param>
        /// <param name="contrasena">Contraseña correspondiente al usuario.</param>
        /// <returns>Una tarea cuyo resultado contiene el mensaje devuelto por la operación de inicio de sesión.</returns>
        Task<string> LoguearAsync(string usuario, string contrasena);

        /// <summary>
        /// Comprueba si existe un administrador con el DNI y tipo de documento indicados.
        /// </summary>
        /// <param name="dni">Número de documento que se desea comprobar.</param>
        /// <param name="tipoDni">Tipo de documento que se desea comprobar.</param>
        /// <returns><see langword="true"/> si existe un administrador con esos datos; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> ExisteDniAsync(string dni, string tipoDni);

        /// <summary>
        /// Comprueba si existe otro administrador con el DNI y tipo de documento indicados,
        /// excluyendo al administrador especificado.
        /// </summary>
        /// <param name="dni">Número de documento que se desea comprobar.</param>
        /// <param name="tipoDni">Tipo de documento que se desea comprobar.</param>
        /// <param name="idExcluir">Identificador del administrador que debe excluirse de la búsqueda.</param>
        /// <returns><see langword="true"/> si existe otro administrador con esos datos; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> ExisteDniAsync(string dni, string tipoDni, int idExcluir);

        /// <summary>
        /// Comprueba si un nombre de usuario ya está registrado.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario que se desea comprobar.</param>
        /// <returns><see langword="true"/> si el nombre de usuario existe; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> ExisteUsuarioAsync(string nombreUsuario);

        /// <summary>
        /// Comprueba si existe un administrador con el identificador indicado.
        /// </summary>
        /// <param name="id">Identificador del administrador que se desea buscar.</param>
        /// <returns><see langword="true"/> si el administrador existe; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> ExistePorIdAsync(int id);

        /// <summary>
        /// Comprueba si un nombre de usuario pertenece a otro administrador,
        /// excluyendo al administrador especificado.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario que se desea comprobar.</param>
        /// <param name="idExcluir">Identificador del administrador que debe excluirse de la búsqueda.</param>
        /// <returns><see langword="true"/> si otro administrador utiliza ese nombre de usuario; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> ExisteUsuarioAsync(string nombreUsuario, int idExcluir);

        /// <summary>
        /// Comprueba si el administrador indicado es el superadministrador.
        /// </summary>
        /// <param name="id">Identificador del administrador que se desea comprobar.</param>
        /// <returns><see langword="true"/> si el administrador es el superadministrador; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> EsSuperAdminAsync(int id);

        /// <summary>
        /// Obtiene los datos de los estudiantes y sus carreras para la exportación.
        /// </summary>
        /// <returns>Una colección de estudiantes con la información académica necesaria para la exportación.</returns>
        Task<IEnumerable<EstudianteExportacionDto>> ListarEstudiantesCarrerasInfoAcaAsync();
    }
}