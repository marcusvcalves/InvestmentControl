namespace InvestmentControl.Domain.Models;

public class Result
{
    protected Result(bool isSuccess, ApiErrorResponse? error = null, object? data = null)
    {
        if (isSuccess && error != null ||
            !isSuccess && error == null)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Data = data;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ApiErrorResponse? Error { get; }

    public object? Data { get; }

    #pragma warning disable IDE0090
    public static Result Success() => new Result(true, null);

    public static Result Success<T>(T data) => new Result(true, null, data);

    public static Result Failure(ApiErrorResponse error) => new Result(false, error);
    #pragma warning restore IDE0090
}
