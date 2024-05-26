namespace Domain;

public class Result<T>
{
    public class UnwrapFailedException(string message, object valueOrError) : Exception(message)
    {
        public object ValueOrError { get; } = valueOrError;
    }

    private const string UnwrapErrorMessage = "Cannot retrieve the value from a failed result.";
    private const string InvalidResultMessage =
        "Result must be either success with a value or failure with an error.";
    private readonly T _value;
    private readonly Error _err;
    private readonly bool _ok;

    public bool IsOk() => _ok;

    public bool IsErr() => !IsOk();

    private Result(bool ok, T value, Error err)
    {
        bool isInvalidResult = ok && err != Error.None || !ok && err == Error.None;
        if (isInvalidResult)
            throw new ArgumentException(InvalidResultMessage);

        _err = err;
        _ok = ok;
        _value = value;
    }

    public bool IsOkAnd(Func<T, bool> predicate) => IsOk() && predicate(_value);

    public T? Ok() => IsOk() ? _value : default;

    public Error? Err() => IsErr() ? _err : default;

    public Result<U> Map<U>(Func<T, U> op) =>
        IsOk() ? Result<U>.Ok(op(_value)) : Result<U>.Err(_err);

    public U MapOr<U>(U defaultValue, Func<T, U> op) => IsOk() ? op(_value) : defaultValue;

    public IEnumerable<T?> Iter() => IsOk() ? [Ok()] : [];

    public T Expect(string message) =>
        IsOk() ? _value : throw new UnwrapFailedException(message, _err);

    public Error ExpectError(string message) =>
        IsErr() ? _err : throw new UnwrapFailedException(message, _value);

    public Error UnwrapError() =>
        IsErr() ? _err : throw new UnwrapFailedException(UnwrapErrorMessage, _value);

    public Result<U> AndThen<U>(Func<T, Result<U>> op) => IsOk() ? op(_value) : Result<U>.Err(_err);

    public T Unwrap() =>
        IsOk() ? _value : throw new UnwrapFailedException(UnwrapErrorMessage, _err);

    public T? UnwrapOrDefault() => IsOk() ? _value : default(T);

    public T UnwrapOr(T defaultValue) => IsOk() ? _value : defaultValue;

    public static Result<T> Ok(T value) => new(true, value, Error.None);

    public static Result<T> Err(Error error) => new(false, default, error);
}
