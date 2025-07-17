using InvestmentControl.Domain.Models;

namespace InvestmentControl.ApplicationCore.Extensions;

public static class ResultExtensions
{
    public static TResult Match<TValue, TResult>(
        this Result result,
        Func<TValue, TResult> onSuccess,
        Func<ApiErrorResponse, TResult> onFailure)                    
    {
        return result.IsSuccess
            ? onSuccess((TValue)result.Data!)
            : onFailure(result.Error!);
    }

    public static TResult Match<TResult>(
        this Result result,
        Func<TResult> onSuccess,
        Func<ApiErrorResponse, TResult> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
    }
}
