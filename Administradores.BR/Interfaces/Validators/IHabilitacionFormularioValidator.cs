using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Formularios.DTOs;

namespace Administradores.BR.Interfaces.Validators
{
    /// <summary>
    /// Define las validaciones de los datos necesarios para crear y editar
    /// formularios de habilitación.
    /// </summary>
    public interface IHabilitacionFormularioValidator
    {
        /// <summary>
        /// Valida los datos necesarios para crear un formulario.
        /// </summary>
        /// <param name="data">Datos del formulario que se desea crear.</param>
        /// <returns>
        /// Un mensaje de error si los datos no son válidos;
        /// null si la validación es exitosa.
        /// </returns>
        string? ValidarCrear(CrearHabilitacionFormularioDto? data);

        /// <summary>
        /// Valida los datos necesarios para editar un formulario.
        /// </summary>
        /// <param name="data">Datos del formulario que se desea editar.</param>
        /// <returns>
        /// Un mensaje de error si los datos no son válidos;
        /// null si la validación es exitosa.
        /// </returns>
        string? ValidarEditar(EditarHabilitacionFormularioDto? data);
    }
}