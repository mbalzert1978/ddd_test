namespace Domain;

public class Result<T>
{
    public class UnwrapFailedException(string message, object valueOrError) : System.Exception(message)
    {
        public object ValueOrError { get; } = valueOrError;
    }

    private const string UwrpErrMsg = "Cannot retrieve the value from a failed result.";
    private const string ConstrErrMsg = "Result must be either success with a value or failure with an error.";
    private readonly T _value;
    private readonly Error _error;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    private Result(bool isSuccess, T value, Error error)
    {
        if (isSuccess
            && error != Error.None
            || !isSuccess
            && error == Error.None) throw new ArgumentException(ConstrErrMsg);

        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    public T Unwrap() =>
     IsSuccess ? _value : throw new UnwrapFailedException(UwrpErrMsg, _error);

    public T? UnwrapOrDefault() =>
     IsSuccess ? _value : default(T);

    public bool Is_Ok() => IsSuccess;

    public bool IsOkAnd(Func<T, bool> predicate) =>
     IsSuccess && predicate(_value);

    public bool Is_Err() => IsFailure;

    public T? Ok() => IsSuccess ? _value : default;

    public Error? Err() => IsFailure ? _error : default;

    public Result<U> Map<U>(Func<T, U> op) =>
     IsSuccess ? Result<U>.Success(op(_value)) : Result<U>.Failure(_error);

    public U MapOr<U>(U defaultValue, Func<T, U> op) =>
     IsSuccess ? op(_value) : defaultValue;

    public IEnumerable<T?> Iter() =>
     IsSuccess ? [Ok()] : [];

    public T Expect(string message) =>
     IsSuccess ? _value : throw new UnwrapFailedException(message, _error);

    public Error ExpectError(string message) =>
     IsFailure ? _error : throw new UnwrapFailedException(message, _value);

    public Error UnwrapError() =>
     IsFailure ? _error : throw new UnwrapFailedException(UwrpErrMsg, _value);

    public Result<U> AndThen<U>(Func<T, Result<U>> op) =>
     IsSuccess ? op(_value) : Result<U>.Failure(_error);
    public T UnwrapOr(T defaultValue) =>
     IsSuccess ? _value : defaultValue;
    public T UnwrapOrElse(Func<Error, T> op) =>
     IsFailure ? op(_error) : _value;
    public static Result<T> Success(T value) =>
     new(true, value, Error.None);

    public static Result<T> Failure(Error error) =>
     new(false, default, error);
}
