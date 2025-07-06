namespace partycli.Models;

public class Result
{
    private readonly string? _errorMessage;

    public bool IsSuccess { get; }

    public string ErrorMessage => !IsSuccess ? _errorMessage! : throw new InvalidOperationException("Result is successful");

    protected Result(bool isSuccess, string? errorMessage)
    {
        _errorMessage = errorMessage;
        IsSuccess = isSuccess;
    }

    public static Result Success() =>
        new(isSuccess: true, null);

    public static Result Failure(string errorMessage) =>
        new(isSuccess: false, errorMessage);

    public static Result<T> Success<T>(T value) =>
        Result<T>.Success(value);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Result is not successful");

    protected Result(bool isSuccess, T? value, string? errorMessage) : base(isSuccess, errorMessage)
    {
        _value = value;
    }

    public static Result<T> Success(T value) =>
        new(isSuccess: true, value, null);

    public new static Result<T> Failure(string errorMessage) =>
        new(isSuccess: false, default, errorMessage);
}