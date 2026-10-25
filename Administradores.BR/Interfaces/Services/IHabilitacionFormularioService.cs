using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Formularios.DTOs;
using Administradores.Models.Formularios.Models;
using Administradores.Models.Results;

namespace Administradores.BR.Interfaces.Services
{

    /// <summary>
    /// Define las operaciones de negocio para gestionar la habilitación de formularios,
    /// incluyendo su creación, modificación, eliminación y consulta de disponibilidad.
    /// </summary>
    public interface IHabilitacionFormularioService
    {
        /// <summary>
        /// Obtiene todos los formularios registrados.
        /// </summary>
        /// <returns>
        /// Un resultado que contiene la colección de formularios si la operación
        /// es exitosa, o un error si ocurre un problema durante la consulta.
        /// </returns>
        Task<Result<IEnumerable<HabilitacionFormulario>>> GetAllAsync();

        /// <summary>
        /// Crea un nuevo formulario después de validar sus datos y comprobar
        /// las restricciones de cantidad y año.
        /// </summary>
        /// <param name="data">
        /// Datos necesarios para crear el formulario, incluyendo el año y las fechas
        /// de inicio y cierre.
        /// </param>
        /// <returns>
        /// Un resultado que indica si la creación fue exitosa o contiene
        /// el error producido durante la operación.
        /// </returns>
        Task<Result<bool>> CrearAsync(CrearHabilitacionFormularioDto data);

        /// <summary>
        /// Modifica las fechas y el estado de un formulario existente,
        /// validando los datos y las restricciones de habilitación.
        /// </summary>
        /// <param name="id">Identificador del formulario que se desea modificar.</param>
        /// <param name="data">
        /// Datos con las nuevas fechas de inicio y cierre y el estado del formulario.
        /// </param>
        /// <returns>
        /// Un resultado que indica si la modificación fue exitosa o contiene
        /// el error producido durante la operación.
        /// </returns>
        Task<Result<bool>> EditarAsync(int id, EditarHabilitacionFormularioDto data);

        /// <summary>
        /// Elimina un formulario existente.
        /// </summary>
        /// <param name="id">Identificador del formulario que se desea eliminar.</param>
        /// <returns>
        /// Un resultado que indica si la eliminación fue exitosa o contiene
        /// el error producido durante la operación.
        /// </returns>
        Task<Result<bool>> EliminarAsync(int id);

        /// <summary>
        /// Consulta el formulario que se encuentra disponible según su estado
        /// y sus fechas de inicio y cierre.
        /// </summary>
        /// <returns>
        /// Un resultado que contiene el formulario disponible, o un valor nulo
        /// si no se encuentra ninguno, o un error si ocurre un problema durante
        /// la consulta.
        /// </returns>
        Task<Result<HabilitacionFormulario>> EstaDisponibleAsync();
    }
}