using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Validators;
using Administradores.Models.Formularios.DTOs;

namespace Administradores.BR.Validadores
{
    /// <summary>
    /// Implementa las validaciones de los datos de los formularios de habilitación.
    /// </summary>
    public class HabilitacionFormularioValidador : IHabilitacionFormularioValidator
    {
        /// <inheritdoc />
        public string? ValidarCrear(CrearHabilitacionFormularioDto? data)
        {
            if (data is null)
            {
                return "Los datos del formulario son obligatorios.";
            }

            if (data.Hab_Form_Año <= 0)
            {
                return "El año del formulario no es válido.";
            }

            var errorFechas = ValidarFechas(
                data.Hab_Form_Fecha_Inicio,
                data.Hab_Form_Fecha_Cierre);

            if (errorFechas is not null)
            {
                return errorFechas;
            }

            return null;
        }

        /// <inheritdoc />
        public string? ValidarEditar(EditarHabilitacionFormularioDto? data)
        {
            if (data is null)
            {
                return "Los datos del formulario son obligatorios.";
            }

            var errorFechas = ValidarFechas(
                data.Hab_Form_Fecha_Inicio,
                data.Hab_Form_Fecha_Cierre);

            if (errorFechas is not null)
            {
                return errorFechas;
            }

            var errorEstado = ValidarEstado(data.Hab_Form_Estado);

            if (errorEstado is not null)
            {
                return errorEstado;
            }

            return null;
        }
        /// <summary>
        /// Valida que las fechas de inicio y cierre del período de inscripción sean válidas.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período de inscripción.</param>
        /// <param name="fechaCierre">Fecha de cierre del período de inscripción.</param>
        /// <returns>
        /// Un mensaje descriptivo si alguna de las fechas no cumple con las condiciones establecidas;
        /// de lo contrario, <see langword="null"/>.
        /// </returns>
        private static string? ValidarFechas(DateTime fechaInicio, DateTime fechaCierre)
        {
            if (fechaInicio == default)
            {
                return "La fecha de inicio es obligatoria.";
            }

            if (fechaCierre == default)
            {
                return "La fecha de cierre es obligatoria.";
            }

            if (fechaInicio >= fechaCierre)
            {
                return "La fecha de inicio debe ser anterior a la fecha de cierre.";
            }

            return null;
        }
        /// <summary>
        /// Valida que el estado del formulario sea obligatorio y corresponda a uno de los valores permitidos.
        /// </summary>
        /// <param name="estado">Estado que se desea validar.</param>
        /// <returns>
        /// Un mensaje descriptivo si el estado no cumple con las condiciones establecidas;
        /// de lo contrario, <see langword="null"/>.
        /// </returns>
        private static string? ValidarEstado(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                return "El estado del formulario es obligatorio.";
            }

            estado = estado.Trim().ToUpperInvariant();

            if (estado != "HABILITADO" && estado != "DESHABILITADO")
            {
                return "El estado debe ser HABILITADO o DESHABILITADO.";
            }

            return null;
        }
    }
}