using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Administradores.DTOs;
using Administradores.Models.Administradores.Models;
using Administradores.Models.Results;

namespace Administradores.BR.Interfaces.Services
{
    /// <summary>
    /// Define las operaciones de negocio disponibles para gestionar administradores,
    /// autenticar usuarios y consultar datos de estudiantes para su exportación.
    /// </summary>
    public interface IAdministradorService
    {
        /// <summary>
        /// Obtiene la lista de administradores registrados en el sistema.
        /// </summary>
        /// <returns>
        /// Un resultado que contiene la colección de administradores si la operación
        /// es exitosa, o información del error si ocurre un problema de persistencia.
        /// </returns>
        Task<Result<IEnumerable<Administrador>>> ListarAsync();

        /// <summary>
        /// Crea un nuevo administrador después de validar sus datos y comprobar
        /// que su DNI y nombre de usuario no estén registrados ni reservados.
        /// </summary>
        /// <param name="admin">
        /// Datos del administrador que se desea crear, incluyendo nombre y apellido,
        /// DNI, tipo de DNI, nombre de usuario y contraseña.
        /// </param>
        /// <returns>
        /// Un resultado que indica si la creación fue exitosa o contiene información
        /// del error producido por validaciones, conflictos o problemas de persistencia.
        /// </returns>
        Task<Result<bool>> CrearAsync(CrearAdminDto admin);

        /// <summary>
        /// Modifica los datos de un administrador existente, validando la información,
        /// verificando que el DNI y el nombre de usuario sean únicos y respetando
        /// las restricciones del superadministrador.
        /// </summary>
        /// <param name="id">
        /// Identificador del administrador que se desea modificar. Debe ser mayor que cero.
        /// </param>
        /// <param name="admin">
        /// Nuevos datos del administrador, incluyendo nombre y apellido, DNI, tipo de DNI,
        /// nombre de usuario y contraseña.
        /// </param>
        /// <returns>
        /// Un resultado que indica si la modificación fue exitosa o contiene información
        /// del error producido por validaciones, un administrador inexistente,
        /// conflictos con los datos o problemas de persistencia.
        /// </returns>
        Task<Result<bool>> EditarAsync(int id, EditarAdminDto admin);

        /// <summary>
        /// Elimina un administrador existente, verificando que el identificador sea válido,
        /// que el administrador exista y que no sea el superadministrador.
        /// </summary>
        /// <param name="id">
        /// Identificador del administrador que se desea eliminar. Debe ser mayor que cero.
        /// </param>
        /// <returns>
        /// Un resultado que indica si la eliminación fue exitosa o contiene información
        /// del error producido por validaciones, un administrador inexistente,
        /// restricciones de eliminación o problemas de persistencia.
        /// </returns>
        Task<Result<bool>> EliminarAsync(int id);

        /// <summary>
        /// Verifica las credenciales de un administrador para permitirle iniciar sesión.
        /// </summary>
        /// <param name="usuario">
        /// Nombre de usuario del administrador que intenta iniciar sesión.
        /// </param>
        /// <param name="contrasena">
        /// Contraseña ingresada para autenticar al administrador.
        /// </param>
        /// <returns>
        /// Un resultado que contiene el mensaje de inicio de sesión exitoso si las
        /// credenciales son correctas, o información del error producido por datos
        /// inválidos, credenciales incorrectas o problemas de persistencia.
        /// </returns>
        Task<Result<string>> LoguearAsync(string usuario, string contrasena);

        /// <summary>
        /// Obtiene los datos de los estudiantes inscriptos para su exportación a Excel.
        /// </summary>
        /// <returns>
        /// Un resultado que contiene la colección de estudiantes para exportar si existen
        /// inscripciones, o información del error producido si no hay inscripciones
        /// o ocurre un problema de persistencia.
        /// </returns>
        Task<Result<IEnumerable<EstudianteExportacionDto>>> ExportarEstudiantesExcelAsync();
    }
}