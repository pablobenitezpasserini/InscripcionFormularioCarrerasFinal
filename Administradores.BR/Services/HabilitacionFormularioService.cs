using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Services;
using Administradores.Models.Exceptions;
using Administradores.Models.Formularios.DTOs;
using Administradores.Models.Formularios.Models;
using Administradores.Models.Results;

namespace Administradores.BR.Services
{
    public class HabilitacionFormularioService : IHabilitacionFormularioService
    {
        private readonly IHabilitacionFormularioRepository _repository;

        public HabilitacionFormularioService(IHabilitacionFormularioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<HabilitacionFormulario>>> GetAllAsync()
        {
            try
            {
                var formularios = await _repository.ListarAsync();

                return Result<IEnumerable<HabilitacionFormulario>>.Ok(formularios);
            }
            catch (DataAccessException)
            {
                return Result<IEnumerable<HabilitacionFormulario>>.Fail(
                    "No se pudieron obtener los formularios por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<bool>> CrearAsync(CrearHabilitacionFormularioDto data)
        {
            if (data is null)
            {
                return Result<bool>.Fail(
                    "Los datos del formulario son obligatorios.",
                    ErrorType.Validation);
            }

            if (data.Hab_Form_Año <= 0)
            {
                return Result<bool>.Fail(
                    "El año del formulario no es válido.",
                    ErrorType.Validation);
            }

            if (data.Hab_Form_Fecha_Inicio == default)
            {
                return Result<bool>.Fail(
                    "La fecha de inicio es obligatoria.",
                    ErrorType.Validation);
            }

            if (data.Hab_Form_Fecha_Cierre == default)
            {
                return Result<bool>.Fail(
                    "La fecha de cierre es obligatoria.",
                    ErrorType.Validation);
            }

            if (data.Hab_Form_Fecha_Inicio >= data.Hab_Form_Fecha_Cierre)
            {
                return Result<bool>.Fail(
                    "La fecha de inicio debe ser anterior a la fecha de cierre.",
                    ErrorType.Validation);
            }

            try
            {
                var formularios = await _repository.ListarAsync();

                if (formularios.Any(f => f.Hab_Form_Año == data.Hab_Form_Año))
                {
                    return Result<bool>.Fail(
                        "Ya existe un formulario para el año ingresado.",
                        ErrorType.Conflict);
                }

                if (formularios.Count() >= 2)
                {
                    return Result<bool>.Fail(
                        "No pueden existir más de dos formularios.",
                        ErrorType.Conflict);
                }

                var formulario = new HabilitacionFormulario
                {
                    Hab_Form_Año = data.Hab_Form_Año,
                    Hab_Form_Fecha_Inicio = data.Hab_Form_Fecha_Inicio,
                    Hab_Form_Fecha_Cierre = data.Hab_Form_Fecha_Cierre,
                    Hab_Form_Estado = "DESHABILITADO"
                };

                await _repository.CrearAsync(formulario);

                return Result<bool>.Ok(true);
            }
            catch (DataAccessException)
            {
                return Result<bool>.Fail(
                    "No se pudo crear el formulario por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<bool>> EditarAsync(int id, EditarHabilitacionFormularioDto data)
        {
            if (id <= 0)
            {
                return Result<bool>.Fail(
                    "El ID del formulario no es válido.",
                    ErrorType.Validation);
            }

            if (data is null)
            {
                return Result<bool>.Fail(
                    "Los datos del formulario son obligatorios.",
                    ErrorType.Validation);
            }

            if (data.Hab_Form_Fecha_Inicio == default ||
                data.Hab_Form_Fecha_Cierre == default)
            {
                return Result<bool>.Fail(
                    "Las fechas de inicio y cierre son obligatorias.",
                    ErrorType.Validation);
            }

            if (data.Hab_Form_Fecha_Inicio >= data.Hab_Form_Fecha_Cierre)
            {
                return Result<bool>.Fail(
                    "La fecha de inicio debe ser anterior a la fecha de cierre.",
                    ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(data.Hab_Form_Estado))
            {
                return Result<bool>.Fail(
                    "El estado del formulario es obligatorio.",
                    ErrorType.Validation);
            }

            var estado = data.Hab_Form_Estado.Trim().ToUpperInvariant();

            if (estado != "HABILITADO" && estado != "DESHABILITADO")
            {
                return Result<bool>.Fail(
                    "El estado debe ser HABILITADO o DESHABILITADO.",
                    ErrorType.Validation);
            }

            try
            {
                var formularios = (await _repository.ListarAsync()).ToList();

                var formularioActual = formularios
                    .FirstOrDefault(f => f.ID_Hab_Form == id);

                if (formularioActual is null)
                {
                    return Result<bool>.Fail(
                        "No existe un formulario con el ID indicado.",
                        ErrorType.NotFound);
                }

                if (estado == "HABILITADO" &&
                    formularios.Any(f =>
                        f.ID_Hab_Form != id &&
                        f.Hab_Form_Estado == "HABILITADO"))
                {
                    return Result<bool>.Fail(
                        "Ya existe otro formulario habilitado.",
                        ErrorType.Conflict);
                }

                var formulario = new HabilitacionFormulario
                {
                    ID_Hab_Form = id,
                    Hab_Form_Año = formularioActual.Hab_Form_Año,
                    Hab_Form_Fecha_Inicio = data.Hab_Form_Fecha_Inicio,
                    Hab_Form_Fecha_Cierre = data.Hab_Form_Fecha_Cierre,
                    Hab_Form_Estado = estado
                };

                await _repository.EditarAsync(formulario);

                return Result<bool>.Ok(true);
            }
            catch (DataAccessException)
            {
                return Result<bool>.Fail(
                    "No se pudo modificar el formulario por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<bool>> EliminarAsync(int id)
        {
            if (id <= 0)
            {
                return Result<bool>.Fail(
                    "El ID del formulario no es válido.",
                    ErrorType.Validation);
            }

            try
            {
                var formularios = await _repository.ListarAsync();

                if (!formularios.Any(f => f.ID_Hab_Form == id))
                {
                    return Result<bool>.Fail(
                        "No existe un formulario con el ID indicado.",
                        ErrorType.NotFound);
                }

                await _repository.EliminarAsync(id);

                return Result<bool>.Ok(true);
            }
            catch (DataAccessException)
            {
                return Result<bool>.Fail(
                    "No se pudo eliminar el formulario por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<HabilitacionFormulario>> EstaDisponibleAsync()
        {
            try
            {
                var formularios = await _repository.ListarAsync();
                var hoy = DateTime.Today;

                var formulario = formularios.Where(f =>
                    string.Equals(f.Hab_Form_Estado, "HABILITADO", StringComparison.OrdinalIgnoreCase) ||
                    (f.Hab_Form_Fecha_Inicio.Date <= hoy && f.Hab_Form_Fecha_Cierre.Date >= hoy))
                    .OrderByDescending(f => string.Equals(f.Hab_Form_Estado, "HABILITADO", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (formulario is null)
                {
                    return Result<HabilitacionFormulario>.Ok(null!);
                }

                return Result<HabilitacionFormulario>.Ok(formulario);
            }
            catch (DataAccessException)
            {
                return Result<HabilitacionFormulario>.Fail(
                    "No se pudo consultar la disponibilidad del formulario por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }
    }
}