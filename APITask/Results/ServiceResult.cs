namespace APITask.Results;

public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public ServiceResultStatus Status { get; }
    public string? Message { get; }
    public T? Data { get; }

    private ServiceResult(bool isSuccess, ServiceResultStatus status, string? message, T? data) // Construtor privado evita a criação de combinações inválidas
    {
        IsSuccess = isSuccess;
        Status = status;
        Message = message;
        Data = data;
    }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(true, ServiceResultStatus.Success, null, data);
    }

    public static ServiceResult<T> NotFound(string message)
    {
        return new ServiceResult<T>(false, ServiceResultStatus.NotFound, message, default);
    }

    public static ServiceResult<T> ValidationError(string message)
    {
        return new ServiceResult<T>(false, ServiceResultStatus.ValidationError, message, default);
    }
}

