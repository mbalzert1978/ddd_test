namespace Domain;

public abstract class Result<T, E>
{
    public class UnwrapFailedException(string message, object valueOrError) : Exception(message)
    {
        public object ValueOrError { get; } = valueOrError;
    }


    public abstract bool IsOk();
    public abstract bool IsOkAnd(Func<T, bool> predicate);
    public abstract bool IsErr();
    public abstract bool IsErrAnd(Func<E, bool> predicate);
    public abstract T? Ok();
    public abstract E? Err();
    public abstract Result<U, E> Map<U>(Func<T, U> op);
    public abstract U MapOr<U>(U defaultValue, Func<T, U> op);
    public abstract U MapOrElse<U>(Func<E, U> f, Func<T, U> op);
    public abstract Result<T, O> MapErr<O>(Func<E, O> op);
    public abstract Result<U, E> AndThen<U>(Func<T, Result<U, E>> op);
    public abstract Result<T, E> OrElse(Func<E, Result<T, E>> op);
    public abstract T Unwrap();
    public abstract E UnwrapErr();
    public abstract T? UnwrapOrDefault();
    public abstract T UnwrapOr(T defaultValue);
    public abstract T UnwrapOrElse(Func<E, T> f);
    public abstract T Expect(string message);
    public abstract E ExpectErr(string message);
    public abstract IEnumerable<T?> Iter();
    public static Result<T, E> Ok(T value) => new OkResult(value);
    public static Result<T, E> Err(E error) => new ErrResult(error);
    private class OkResult(T value) : Result<T, E>
    {
        private const string UnwrapErrorMessage = "Cannot unwrap error from a success result.";

        private T Value { get; } = value;

        public static bool operator ==(OkResult? left, OkResult? right)
        {
            if (left is null || right is null)
                return false;

            return Equals(left, right);
        }

        public static bool operator !=(OkResult? left, OkResult? right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            if (obj is not OkResult result)
                return false;

            return Equals(Value, result.Value);
        }

        public bool Equals(OkResult? other)
        {
            if (other is null)
                return false;
            if (other.GetType() != GetType())
                return false;

            return Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() * 41;
        }

        public override bool IsOk() => true;
        public override bool IsOkAnd(Func<T, bool> predicate) => predicate(Value);
        public override bool IsErr() => false;
        public override bool IsErrAnd(Func<E, bool> predicate) => false;
        public override T? Ok() => Value;
        public override E? Err() => default;
        public override Result<U, E> Map<U>(Func<T, U> op) => Result<U, E>.Ok(op(Value));
        public override U MapOr<U>(U defaultValue, Func<T, U> op) => op(Value);
        public override U MapOrElse<U>(Func<E, U> f, Func<T, U> op) => op(Value);
        public override Result<T, O> MapErr<O>(Func<E, O> op) => Result<T, O>.Ok(Value);
        public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> op) => op(Value);
        public override Result<T, E> OrElse(Func<E, Result<T, E>> op) => Result<T, E>.Ok(Value);
        public override IEnumerable<T?> Iter() => [Value];
        public override T Unwrap() => Value;
        public override E UnwrapErr() => throw new UnwrapFailedException(UnwrapErrorMessage, Value);
        public override T? UnwrapOrDefault() => Value;
        public override T UnwrapOr(T defaultValue) => Value;
        public override T UnwrapOrElse(Func<E, T> f) => Value;
        public override T Expect(string message) => Value;
        public override E ExpectErr(string message) => throw new UnwrapFailedException(message, Value);


    }

    private class ErrResult(E error) : Result<T, E>
    {
        private const string UnwrapErrorMessage = "Cannot unwrap a failure result.";

        private E Error { get; } = error;

        public static bool operator ==(ErrResult? left, ErrResult? right)
        {
            if (left is null || right is null)
                return false;

            return Equals(left, right);
        }

        public static bool operator !=(ErrResult? left, ErrResult? right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            if (obj is not ErrResult result)
                return false;

            return Equals(Error, result.Error);
        }

        public bool Equals(ErrResult? other)
        {
            if (other is null)
                return false;
            if (other.GetType() != GetType())
                return false;

            return Equals(Error, other.Error);
        }

        public override int GetHashCode()
        {
            return Error.GetHashCode() * 41;
        }


        public override bool IsOk() => false;
        public override bool IsOkAnd(Func<T, bool> predicate) => false;
        public override bool IsErr() => true;
        public override bool IsErrAnd(Func<E, bool> predicate) => predicate(Error);
        public override T? Ok() => default;
        public override E? Err() => Error;
        public override Result<U, E> Map<U>(Func<T, U> op) => Result<U, E>.Err(Error);
        public override U MapOr<U>(U defaultValue, Func<T, U> op) => defaultValue;
        public override U MapOrElse<U>(Func<E, U> f, Func<T, U> op) => f(Error);
        public override Result<T, O> MapErr<O>(Func<E, O> op) => Result<T, O>.Err(op(Error));
        public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> op) => Result<U, E>.Err(Error);
        public override Result<T, E> OrElse(Func<E, Result<T, E>> op) => op(Error);
        public override IEnumerable<T?> Iter() => [];
        public override T Unwrap() => throw new UnwrapFailedException(UnwrapErrorMessage, Error);
        public override E UnwrapErr() => Error;
        public override T? UnwrapOrDefault() => default(T);
        public override T UnwrapOr(T defaultValue) => defaultValue;
        public override T UnwrapOrElse(Func<E, T> f) => f(Error);
        public override T Expect(string message) => throw new UnwrapFailedException(message, Error);
        public override E ExpectErr(string message) => Error;
    }
}
