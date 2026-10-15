using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Results
{
public class Result<T>
{
    public bool Success { get; }
    public T? Data { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }

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

    public static Result<T> Ok(T data)
        => new(true, data, null, ErrorType.None);

    public static Result<T> Fail(
        string error,
        ErrorType errorType)
        => new(false, default, error, errorType);
}
}