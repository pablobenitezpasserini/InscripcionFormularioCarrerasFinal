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

            if (data.Hab_Form_Fecha_Inicio == default)
            {
                return "La fecha de inicio es obligatoria.";
            }

            if (data.Hab_Form_Fecha_Cierre == default)
            {
                return "La fecha de cierre es obligatoria.";
            }

            if (data.Hab_Form_Fecha_Inicio >= data.Hab_Form_Fecha_Cierre)
            {
                return "La fecha de inicio debe ser anterior a la fecha de cierre.";
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

            if (data.Hab_Form_Fecha_Inicio == default ||
                data.Hab_Form_Fecha_Cierre == default)
            {
                return "Las fechas de inicio y cierre son obligatorias.";
            }

            if (data.Hab_Form_Fecha_Inicio >= data.Hab_Form_Fecha_Cierre)
            {
                return "La fecha de inicio debe ser anterior a la fecha de cierre.";
            }

            if (string.IsNullOrWhiteSpace(data.Hab_Form_Estado))
            {
                return "El estado del formulario es obligatorio.";
            }

            var estado = data.Hab_Form_Estado.Trim().ToUpperInvariant();

            if (estado != "HABILITADO" && estado != "DESHABILITADO")
            {
                return "El estado debe ser HABILITADO o DESHABILITADO.";
            }

            return null;
        }
    }
}