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
    public class AdministradorRepository : IAdministradorRepository
    {
        private readonly string _connectionString;

        public AdministradorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

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

        public Task<bool> EsSuperAdminAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteDniAsync(string dni, string tipoDni)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteDniAsync(string dni, string tipoDni, int idExcluir)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistePorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteUsuarioAsync(string nombreUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteUsuarioAsync(string nombreUsuario, int idExcluir)
        {
            throw new NotImplementedException();
        }

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