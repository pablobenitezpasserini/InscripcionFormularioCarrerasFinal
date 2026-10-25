using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Repositories;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Exceptions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Administradores.DA.Repositories
{
    /// <summary>
    /// Implementa las operaciones de acceso a datos de los administradores
    /// y la consulta de estudiantes para exportación mediante procedimientos almacenados.
    /// </summary>
    public class AdministradorRepository : IAdministradorRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="AdministradorRepository"/>
        /// con la cadena de conexión a la base de datos.
        /// </summary>
        /// <param name="connectionString">
        /// Cadena de conexión utilizada para establecer conexiones con la base de datos.
        /// </param>
        public AdministradorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public async Task CrearAsync(Administrador admin)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Admin_Nom_Ape", admin.Admin_Nom_Ape);
                parameters.Add("@Admin_DNI", admin.Admin_DNI);
                parameters.Add("@Admin_Tipo_DNI", admin.Admin_Tipo_DNI);
                parameters.Add("@Admin_Nom_Usuario", admin.Admin_Nom_Usuario);
                parameters.Add("@Admin_Contraseña", admin.Admin_Contra);

                await connection.ExecuteAsync(
                    "Agregar_Admin",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al crear el administrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task EditarAsync(Administrador admin)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@ID_Admin", admin.ID_Admin);
                parameters.Add("@Nuevo_Admin_Nom_Ape", admin.Admin_Nom_Ape);
                parameters.Add("@Nuevo_Admin_DNI", admin.Admin_DNI);
                parameters.Add("@Nuevo_Admin_Tipo_DNI", admin.Admin_Tipo_DNI);
                parameters.Add("@Nuevo_Admin_Nom_Usuario", admin.Admin_Nom_Usuario);
                parameters.Add("@Nuevo_Admin_Contra", admin.Admin_Contra);

                await connection.ExecuteAsync(
                    "Admin_Update",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al modificar el administrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@ID_Admin", id);

                await connection.ExecuteAsync(
                    "Admin_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al eliminar el administrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> EsSuperAdminAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Id", id);

                return await connection.QuerySingleAsync<bool>(
                    "SP_Administrador_EsSuperAdmin",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al verificar si el administrador es superadministrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExisteDniAsync(string dni, string tipoDni)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Dni", dni);
                parameters.Add("@TipoDni", tipoDni);

                return await connection.QuerySingleAsync<bool>(
                    "SP_Administrador_ExisteDni",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al verificar si existe el DNI del administrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExisteDniAsync(string dni, string tipoDni, int idExcluir)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Dni", dni);
                parameters.Add("@TipoDni", tipoDni);
                parameters.Add("@IdExcluir", idExcluir);

                return await connection.QuerySingleAsync<bool>(
                    "SP_Administrador_ExisteDni_Excluir",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al verificar si existe el DNI del administrador excluyendo su ID.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExistePorIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Id", id);

                return await connection.QuerySingleAsync<bool>(
                    "SP_Administrador_ExistePorId",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al verificar si existe el administrador por su ID.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExisteUsuarioAsync(string nombreUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@NombreUsuario", nombreUsuario);

                return await connection.QuerySingleAsync<bool>(
                    "SP_Administrador_ExisteUsuario",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al verificar si existe el nombre de usuario del administrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExisteUsuarioAsync(string nombreUsuario, int idExcluir)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@NombreUsuario", nombreUsuario);
                parameters.Add("@IdExcluir", idExcluir);

                return await connection.QuerySingleAsync<bool>(
                    "SP_Administrador_ExisteUsuario_Excluir",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al verificar si existe el nombre de usuario del administrador excluyendo su ID.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Administrador>> ListarAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var administradores = await connection.QueryAsync<Administrador>(
                    "sp_Listar_Admin",
                    commandType: CommandType.StoredProcedure);

                return administradores;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al listar los administradores.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<string> LoguearAsync(string usuario, string contrasena)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Admin_Nom_Usuario", usuario);
                parameters.Add("@Admin_Contra", contrasena);
                parameters.Add(
                    "@Mensaje",
                    dbType: DbType.String,
                    size: 100,
                    direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "Logueo_Admin",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<string>("@Mensaje") ?? string.Empty;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Error al consultar las credenciales del administrador.",
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EstudianteExportacionDto>> ListarEstudiantesCarrerasInfoAcaAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var estudiantes = await connection.QueryAsync<EstudianteExportacionDto>(
                    "SP_ExportacionDelExcel",
                    commandType: CommandType.StoredProcedure
                );

                return estudiantes;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Error al obtener los estudiantes para la exportación.", ex);
            }
        }
    }
}