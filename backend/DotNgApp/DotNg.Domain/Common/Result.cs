using DotNg.Domain.Common.Errors;

namespace DotNg.Domain.Common;

public class Result
{
    public Error? Error { get; protected set; }

    public bool IsFailed => Error != null;

    public bool IsSuccess => !IsFailed;

    public static Result Fail(Error error)
    {
        return new Result
        {
            Error = error
        };
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result<T> Success<T>(T value) => new(value);
    public static Result<T> Fail<T>(Error error) => new(error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    public Result(T? value)
    {
        Value = value;
    }

    public Result(Error error)
    {
        Error = error;
    }

    public new static Result<T> Fail(Error error)
    {
        return new Result<T>(error);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }
}