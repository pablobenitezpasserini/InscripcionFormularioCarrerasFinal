using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Repositories;
using Administradores.BR.Interfaces.Services;
using Administradores.BR.Interfaces.Validators;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Exceptions;
using Administradores.Models.Results;

namespace Administradores.BR.Services
{
    public class AdministradorService : IAdministradorService
    {
        private readonly IAdministradorValidator _validador;
        private readonly IAdministradorRepository _adminRepository;
        public AdministradorService(IAdministradorValidator validador, IAdministradorRepository administradorRepository)
        {
            this._validador = validador;
            this._adminRepository = administradorRepository;
        }
        public async Task<Result<IEnumerable<Administrador>>> ListarAsync()
        {
            try
            {
                var administradores = await _adminRepository.ListarAsync();

                return Result<IEnumerable<Administrador>>.Ok(administradores);
            }
            catch (DataAccessException)
            {
                return Result<IEnumerable<Administrador>>.Fail(
                    "No se pudieron obtener los administradores por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<bool>> CrearAsync(CrearAdminDto admin)
        {
            var errorValidacion = _validador.ValidarCrear(admin);

            if (errorValidacion is not null)
            {
                return Result<bool>.Fail(
                    errorValidacion,
                    ErrorType.Validation);
            }

            var nombre = admin.Admin_Nom_Ape!.Trim();
            var dni = admin.Admin_DNI!.Trim();
            var tipoDni = admin.Admin_Tipo_DNI!.Trim();
            var usuario = admin.Admin_Nom_Usuario!.Trim();
            var contraseña = admin.Admin_Contra!;

            // Reserva del admin
            if (string.Equals(usuario, "admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return Result<bool>.Fail(
                    "El nombre de usuario \"admin\" está reservado para el super administrador.", ErrorType.Conflict);
            }

            if (dni == "00000000" && string.Equals(tipoDni, "DNI", StringComparison.OrdinalIgnoreCase))
            {
                return Result<bool>.Fail("El DNI 00000000 está reservado para el super administrador.", ErrorType.Conflict);
            }

            try
            {
                if (await _adminRepository.ExisteDniAsync(dni, tipoDni))
                {
                    return Result<bool>.Fail("Ya existe un administrador con ese DNI y tipo de DNI.", ErrorType.Conflict);
                }

                //Validar nombre de usuario único
                if (await _adminRepository.ExisteUsuarioAsync(usuario))
                {
                    return Result<bool>.Fail("El nombre de usuario ingresado ya existe.", ErrorType.Conflict);
                }

                var nuevoAdmin = new Administrador
                {
                    Admin_Nom_Ape = nombre,
                    Admin_DNI = dni,
                    Admin_Tipo_DNI = tipoDni,
                    Admin_Nom_Usuario = usuario,
                    Admin_Contra = contraseña
                };

                await _adminRepository.CrearAsync(nuevoAdmin);

                return Result<bool>.Ok(true);
            }
            catch (DataAccessException)
            {
                return Result<bool>.Fail(
                    "No se pudo crear el administrador por un error de base de datos.", ErrorType.Unexpected);
            }
        }

        public async Task<Result<bool>> EditarAsync(int id, EditarAdminDto admin)
        {
            if (id <= 0)
            {
                return Result<bool>.Fail(
                    "El ID del administrador no es válido.",
                    ErrorType.Validation);
            }

            var errorValidacion = _validador.ValidarEditar(admin);

            if (errorValidacion is not null)
            {
                return Result<bool>.Fail(
                    errorValidacion,
                    ErrorType.Validation);
            }

            var nombre = admin.Admin_Nom_Ape!.Trim();
            var dni = admin.Admin_DNI!.Trim();
            var tipoDni = admin.Admin_Tipo_DNI!.Trim();
            var usuario = admin.Admin_Nom_Usuario!.Trim();
            var contraseña = admin.Admin_Contra!;

            try
            {
                if (!await _adminRepository.ExistePorIdAsync(id))
                {
                    return Result<bool>.Fail(
                        "No existe un administrador con el ID indicado.",
                        ErrorType.NotFound);
                }

                var esSuperAdmin = await _adminRepository.EsSuperAdminAsync(id);

                if (!esSuperAdmin)
                {
                    if (string.Equals(
                        usuario,
                        "admin",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return Result<bool>.Fail(
                            "El nombre de usuario admin está reservado.",
                            ErrorType.Conflict);
                    }

                    if (dni == "00000000" &&
                        string.Equals(
                            tipoDni,
                            "DNI",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return Result<bool>.Fail(
                            "El DNI 00000000 está reservado.",
                            ErrorType.Conflict);
                    }
                }
                else
                {
                    if (dni != "00000000" || !string.Equals(tipoDni, "DNI", StringComparison.OrdinalIgnoreCase))
                    {
                        return Result<bool>.Fail("No se puede modificar el DNI ni el tipo de DNI del super administrador.", ErrorType.Conflict);
                    }

                    if (!string.Equals(usuario, "admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return Result<bool>.Fail("No se puede modificar el nombre de usuario del super administrador.", ErrorType.Conflict);
                    }
                }

                if (await _adminRepository.ExisteDniAsync(
                    dni, tipoDni, id))
                {
                    return Result<bool>.Fail(
                        "Ya existe otro administrador con ese DNI y tipo de DNI.",
                        ErrorType.Conflict);
                }

                if (await _adminRepository.ExisteUsuarioAsync(
                    usuario, id))
                {
                    return Result<bool>.Fail(
                        "El nombre de usuario ya pertenece a otro administrador.",
                        ErrorType.Conflict);
                }

                var administrador = new Administrador
                {
                    ID_Admin = id,
                    Admin_Nom_Ape = nombre,
                    Admin_DNI = dni,
                    Admin_Tipo_DNI = tipoDni,
                    Admin_Nom_Usuario = usuario,
                    Admin_Contra = contraseña
                };

                await _adminRepository.EditarAsync(administrador);

                return Result<bool>.Ok(true);
            }
            catch (DataAccessException)
            {
                return Result<bool>.Fail(
                    "No se pudo modificar el administrador por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<bool>> EliminarAsync(int id)
        {
            if (id <= 0)
            {
                return Result<bool>.Fail(
                    "El ID del administrador no es válido.",
                    ErrorType.Validation);
            }

            try
            {
                if (!await _adminRepository.ExistePorIdAsync(id))
                {
                    return Result<bool>.Fail(
                        "No existe un administrador con el ID indicado.",
                        ErrorType.NotFound);
                }

                if (await _adminRepository.EsSuperAdminAsync(id))
                {
                    return Result<bool>.Fail(
                        "El superadministrador no puede ser eliminado.",
                        ErrorType.Conflict);
                }

                await _adminRepository.EliminarAsync(id);

                return Result<bool>.Ok(true);
            }
            catch (DataAccessException)
            {
                return Result<bool>.Fail(
                    "No se pudo eliminar el administrador por un error de persistencia.",
                    ErrorType.Unexpected);
            }
        }

        public async Task<Result<string>> LoguearAsync(string usuario, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return Result<string>.Fail(
                    "El nombre de usuario es obligatorio.",
                    ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                return Result<string>.Fail(
                    "La contraseña es obligatoria.",
                    ErrorType.Validation);
            }

            try
            {
                var mensaje = await _adminRepository.LoguearAsync(
                    usuario.Trim(),
                    contrasena);

                if (mensaje.Contains("Logueo exitoso", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<string>.Ok(mensaje);
                }

                return Result<string>.Fail("Nombre de usuario o contraseña incorrectos.", ErrorType.Unauthorized);
            }
            catch (DataAccessException)
            {
                return Result<string>.Fail("No se pudo verificar el acceso por un error de persistencia.", ErrorType.Unexpected);
            }
        }

        public async Task<Result<IEnumerable<EstudianteExportacionDto>>> ExportarEstudiantesExcelAsync()
        {
            try
            {
                var estudiantes = (await _adminRepository.ListarEstudiantesCarrerasInfoAcaAsync()).ToList();

                if (!estudiantes.Any())
                {
                    return Result<IEnumerable<EstudianteExportacionDto>>.Fail("No hay inscripciones hechas.", ErrorType.NotFound);
                }

                return Result<IEnumerable<EstudianteExportacionDto>>.Ok(estudiantes);
            }
            catch (DataAccessException)
            {
                return Result<IEnumerable<EstudianteExportacionDto>>.Fail(
                    "No se pudieron obtener los estudiantes para la exportación por un error de persistencia.",
                    ErrorType.Unexpected
                );
            }
        }
    }
}