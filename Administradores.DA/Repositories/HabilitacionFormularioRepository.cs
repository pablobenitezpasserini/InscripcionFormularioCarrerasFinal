using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Administradores.BR.Interfaces.Repositories;
using Administradores.Models.Exceptions;
using Administradores.Models.Formularios.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Administradores.DA.Repositories
{
    public class HabilitacionFormularioRepository : IHabilitacionFormularioRepository
    {
        private readonly string _connectionString;

        public HabilitacionFormularioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<HabilitacionFormulario>> ListarAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryAsync<HabilitacionFormulario>(
                    "sp_Listar_Habilitacion_Formulario",
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(
                    "Error al obtener los formularios.",
                    ex);
            }
        }

        public async Task CrearAsync(HabilitacionFormulario formulario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@Hab_Form_Año", formulario.Hab_Form_Año);
                parameters.Add(
                    "@Hab_Form_Fecha_Inicio",
                    formulario.Hab_Form_Fecha_Inicio);
                parameters.Add(
                    "@Hab_Form_Fecha_Cierre",
                    formulario.Hab_Form_Fecha_Cierre);

                await connection.ExecuteAsync(
                    "sp_Insert_Habilitacion_Formulario",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(
                    "Error al crear el formulario.",
                    ex);
            }
        }

        public async Task EditarAsync(HabilitacionFormulario formulario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@ID_Hab_Form", formulario.ID_Hab_Form);
                parameters.Add(
                    "@Hab_Form_Fecha_Inicio",
                    formulario.Hab_Form_Fecha_Inicio);
                parameters.Add(
                    "@Hab_Form_Fecha_Cierre",
                    formulario.Hab_Form_Fecha_Cierre);
                parameters.Add(
                    "@Hab_Form_Estado",
                    formulario.Hab_Form_Estado);

                await connection.ExecuteAsync(
                    "sp_Update_Habilitacion_Formulario",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(
                    "Error al modificar el formulario.",
                    ex);
            }
        }

        public async Task EliminarAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@ID_Hab_Form", id);

                await connection.ExecuteAsync(
                    "sp_Delete_Habilitacion_Formulario",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(
                    "Error al eliminar el formulario.",
                    ex);
            }
        }
    }
}