using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Services;
using Administradores.BR.Interfaces.Validators;
using Administradores.Models.Exceptions;
using Administradores.Models.Formularios.DTOs;
using Administradores.Models.Formularios.Models;
using Administradores.Models.Results;

namespace Administradores.BR.Services
{
    /// <summary>
    /// Implementa la lógica de negocio para gestionar los formularios,
    /// validar sus datos y consultar su disponibilidad.
    /// </summary>
    public class HabilitacionFormularioService : IHabilitacionFormularioService
    {
        private readonly IHabilitacionFormularioRepository _repository;
        private readonly IHabilitacionFormularioValidator _validador;

        /// <summary>
        /// Inicializa una nueva instancia de
        /// <see cref="HabilitacionFormularioService"/>.
        /// </summary>
        /// <param name="repository">
        /// Repositorio encargado de gestionar los formularios.
        /// </param>
        /// <param name="validador">
        /// Validador encargado de comprobar los datos de los formularios.
        /// </param>
        public HabilitacionFormularioService(IHabilitacionFormularioRepository repository, IHabilitacionFormularioValidator validador)
        {
            _repository = repository;
            _validador = validador;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<Result<bool>> CrearAsync(CrearHabilitacionFormularioDto data)
        {
            var errorValidacion = _validador.ValidarCrear(data);

            if (errorValidacion is not null)
            {
                return Result<bool>.Fail(
                    errorValidacion,
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

        /// <inheritdoc />
        public async Task<Result<bool>> EditarAsync(int id, EditarHabilitacionFormularioDto data)
        {
            if (id <= 0)
            {
                return Result<bool>.Fail(
                    "El ID del formulario no es válido.",
                    ErrorType.Validation);
            }

            var errorValidacion = _validador.ValidarEditar(data);

            if (errorValidacion is not null)
            {
                return Result<bool>.Fail(
                    errorValidacion,
                    ErrorType.Validation);
            }

            var estado = data.Hab_Form_Estado.Trim().ToUpperInvariant();

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

        /// <inheritdoc />
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

        /// <inheritdoc />
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