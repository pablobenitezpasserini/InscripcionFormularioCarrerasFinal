namespace Administradores.Models.Results
{
    /// <summary>
    /// Define los tipos de errores que pueden producirse durante el procesamiento de las operaciones del sistema.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Indica que no se produjo ningún error.
        /// </summary>
        None,
        /// <summary>
        /// Indica que los datos proporcionados no cumplen con las reglas de validación.
        /// </summary>
        Validation,
        /// <summary>
        /// Indica que el recurso solicitado no existe o no fue encontrado.
        /// </summary>
        NotFound,
        /// <summary>
        /// Indica que la operación no puede realizarse debido a un conflicto con reglas de negocio del sistema.
        /// </summary>
        Conflict,
        /// <summary>
        /// Indica que la solicitud requiere autenticación o que las credenciales proporcionadas no son válidas.
        /// </summary>
        Unauthorized,
        /// <summary>
        /// Indica que el usuario autenticado no tiene permisos para realizar la operación solicitada.
        /// </summary>
        Forbidden,
        /// <summary>
        /// Indica que se produjo un error inesperado durante el procesamiento de la operación.
        /// </summary>
        Unexpected
    }
}