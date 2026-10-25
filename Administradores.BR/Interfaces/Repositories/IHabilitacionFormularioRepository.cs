using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Formularios.Models;

namespace Administradores.BR.Interfaces.Repositories
{
    /// <summary>
    /// Define las operaciones de acceso a datos para gestionar los formularios
    /// de habilitación.
    /// </summary>
    public interface IHabilitacionFormularioRepository
    {
        /// <summary>
        /// Obtiene todos los formularios de habilitación registrados.
        /// </summary>
        /// <returns>
        /// Una colección de formularios registrados en el sistema.
        /// </returns>
        Task<IEnumerable<HabilitacionFormulario>> ListarAsync();

        /// <summary>
        /// Registra un nuevo formulario de habilitación.
        /// </summary>
        /// <param name="formulario">
        /// Formulario con los datos que se desean registrar.
        /// </param>
        /// <returns>
        /// Una tarea que representa la operación asíncrona.
        /// </returns>
        Task CrearAsync(HabilitacionFormulario formulario);

        /// <summary>
        /// Actualiza los datos de un formulario de habilitación existente.
        /// </summary>
        /// <param name="formulario">
        /// Formulario con el identificador y los datos que se desean actualizar.
        /// </param>
        /// <returns>
        /// Una tarea que representa la operación asíncrona.
        /// </returns>
        Task EditarAsync(HabilitacionFormulario formulario);

        /// <summary>
        /// Elimina un formulario de habilitación por su identificador.
        /// </summary>
        /// <param name="id">
        /// Identificador del formulario que se desea eliminar.
        /// </param>
        /// <returns>
        /// Una tarea que representa la operación asíncrona.
        /// </returns>
        Task EliminarAsync(int id);
    }
}