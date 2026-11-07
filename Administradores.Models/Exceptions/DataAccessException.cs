using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Exceptions
{
    /// <summary>
    /// Representa una excepción producida durante una operación de acceso a datos.
    /// Permite conservar la excepción original que provocó el error.
    /// </summary>
    public class DataAccessException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la excepción de acceso a datos.
        /// </summary>
        /// <param name="message">Mensaje que describe el error ocurrido.</param>
        /// <param name="innerException">Excepción original que provocó el error.</param>
        public DataAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}