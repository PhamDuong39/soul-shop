using Microsoft.AspNetCore.Http;

namespace Soul.Shop.Infrastructure;

public class Result<TValue> : Result
{
    public TValue Data { get; set; }

    protected internal Result(TValue value, bool success, string message, int statusCode)
        : base(success, message, statusCode)
    {
        Data = value;
    }
}
