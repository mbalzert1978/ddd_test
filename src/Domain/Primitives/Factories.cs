namespace Domain.Primitives;

public static class Factories {
    public static Result<T, E> Ok<T, E>(T value)
        where T : notnull => new(value);

    public static Result<T, string> Ok<T>(T value)
        where T : notnull => new(value);

    public static Result<T, E> Err<T, E>(E error)
        where T : notnull => new(error);

    public static Result<T, string> Err<T>(string error)
        where T : notnull => new(error);

    public static Result<T, Error> Err<T>(Error error)
        where T : notnull => new(error);

    public static Result<T, Exception> Try<T>(Func<T> op)
        where T : notnull {
        try {
            return Ok<T, Exception>(op());
        } catch (Exception ex) {
            return Err<T, Exception>(ex);
        }
    }
}
