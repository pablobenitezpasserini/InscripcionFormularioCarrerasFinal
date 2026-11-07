using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Results
{
    /// <summary>
    /// Representa el resultado de una operación del sistema, indicando si fue exitosa
    /// y proporcionando los datos obtenidos o la información del error producido.
    /// </summary>
    /// <typeparam name="T">Tipo de dato que contiene el resultado de la operación.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Indica si la operación se completó correctamente.
        /// </summary>
        public bool Success { get; }
        /// <summary>
        /// Datos obtenidos como resultado de una operación exitosa.
        /// </summary>
        public T? Data { get; }
        /// <summary>
        /// Mensaje descriptivo del error, en caso de que la operación haya fallado.
        /// </summary>
        public string? Error { get; }
        /// <summary>
        /// Tipo de error producido durante la operación.
        /// </summary>
        public ErrorType ErrorType { get; }
        /// <summary>
        /// Inicializa una nueva instancia del resultado con su estado, datos y posible información de error.
        /// </summary>
        /// <param name="success">Indica si la operación fue exitosa.</param>
        /// <param name="data">Datos asociados al resultado.</param>
        /// <param name="error">Mensaje descriptivo del error, si corresponde.</param>
        /// <param name="errorType">Clasificación del error producido.</param>
        private Result(
            bool success,
            T? data,
            string? error,
            ErrorType errorType)
        {
            Success = success;
            Data = data;
            Error = error;
            ErrorType = errorType;
        }
        /// <summary>
        /// Crea un resultado que representa una operación exitosa.
        /// </summary>
        /// <param name="data">Datos obtenidos durante la operación.</param>
        /// <returns>Una instancia de <see cref="Result{T}"/> con estado exitoso.</returns>
        public static Result<T> Ok(T data)
            => new(true, data, null, ErrorType.None);
        /// <summary>
        /// Crea un resultado que representa una operación fallida.
        /// </summary>
        /// <param name="error">Mensaje que describe el motivo del fallo.</param>
        /// <param name="errorType">Tipo de error producido.</param>
        /// <returns>Una instancia de <see cref="Result{T}"/> con estado fallido.</returns>
        public static Result<T> Fail(
            string error,
            ErrorType errorType)
            => new(false, default, error, errorType);
    }
}