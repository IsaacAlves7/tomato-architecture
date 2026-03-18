namespace SharedKernel.Common;

/// <summary>
/// Padrão Result para retornos sem exceções.
/// Evita o uso de exceções para controle de fluxo.
/// </summary>
public class Result<T>
{
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    private Result(string error)
    {
        Error = error;
        IsSuccess = false;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
}

public class Result
{
    public string? Error { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    private Result(bool success, string? error = null)
    {
        IsSuccess = success;
        Error = error;
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
}
