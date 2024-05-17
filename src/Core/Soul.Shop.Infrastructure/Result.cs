using Microsoft.AspNetCore.Http;

namespace Soul.Shop.Infrastructure;

public class Result
{
    public bool Success { get; }

    public string Message { get; }

    public int StatusCode { get; set; }


    protected Result(bool success, string message, int statusCode)
    {
        Success = success;
        Message = message;
        StatusCode = statusCode;
    }

    public static Result Fail(string error)
    {
        return new Result(false, error, StatusCodes.Status400BadRequest);
    }


    public static Result Fail<TValue>(TValue value, string error, int statusCode)
    {
        return new Result<TValue>(value, false, error, statusCode);
    }


    public static Result SystemError(string error)
    {
        return new Result(false, error, StatusCodes.Status500InternalServerError);
    }

    public static Result SystemError<TValue>(TValue value, string error)
    {
        return new Result<TValue>(value, false, error, StatusCodes.Status500InternalServerError);
    }

    public static Result Ok()
    {
        return new Result(true, "", StatusCodes.Status200OK);
    }

    public static Result<TValue> Ok<TValue>(TValue value)
    {
        return new Result<TValue>(value, true, "", StatusCodes.Status200OK);
    }

    public static Result<TValue> Ok<TValue>(TValue value, string message)
    {
        return new Result<TValue>(value, true, message, StatusCodes.Status200OK);
    }

    public static Result<TValue> Fail<TValue>(string error)
    {
        return new Result<TValue>(default, false, error, StatusCodes.Status400BadRequest);
    }
}
