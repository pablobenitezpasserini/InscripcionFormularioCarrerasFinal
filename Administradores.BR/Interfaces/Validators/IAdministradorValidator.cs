using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Administradores.DTOs;

namespace Administradores.BR.Interfaces.Validators
{
    /// <summary>
    /// Define las operaciones de validación de los datos de los administradores
    /// para su creación y modificación.
    /// </summary>
    public interface IAdministradorValidator
    {
        /// <summary>
        /// Valida los datos necesarios para crear un administrador.
        /// </summary>
        /// <param name="admin">
        /// Datos del administrador que se desea crear. Puede ser nulo.
        /// </param>
        /// <returns>
        /// Un mensaje descriptivo del error si los datos no son válidos;
        /// null si todas las validaciones son satisfactorias.
        /// </returns>
        public string? ValidarCrear(CrearAdminDto? admin);
        /// <summary>
        /// Valida los datos necesarios para modificar un administrador existente.
        /// </summary>
        /// <param name="admin">
        /// Datos del administrador que se desea modificar. Puede ser nulo.
        /// </param>
        /// <returns>
        /// Un mensaje descriptivo del error si los datos no son válidos;
        /// null si todas las validaciones son satisfactorias.
        /// </returns>
        public string? ValidarEditar(EditarAdminDto? admin);
    }
}