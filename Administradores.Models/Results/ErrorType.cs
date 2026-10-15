namespace Administradores.Models.Results
{
    public enum ErrorType
    {
        None,
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden,
        Unexpected
    }
}