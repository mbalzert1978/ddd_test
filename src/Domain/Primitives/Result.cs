using System.Diagnostics.CodeAnalysis;

namespace Domain.Primitives;

public abstract class Result<T, E>
    where T : notnull
    where E : notnull {
    private const string NotNullMessage =
        "The function provided to Result.Try returned null, which violates the non-null constraint.";

    public class UnwrapFailedException : Exception {
        public object? ValueOrError { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnwrapFailedException"/> class with a specified error message and value.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="value">The value associated with the exception.</param>
        public UnwrapFailedException(string message, T? value)
            : base(message) {
            ValueOrError = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnwrapFailedException"/> class with a specified error message and error.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="error">The error associated with the exception.</param>
        public UnwrapFailedException(string message, E? error)
            : base(message) {
            ValueOrError = error;
        }
    }

    /// <summary>
    /// Determines whether the result is an Ok result.
    /// </summary>
    /// <returns>True if the result is Ok; otherwise, false.</returns>
    public abstract bool IsOk([MaybeNullWhen(false)] out T value);

    /// <summary>
    /// Determines whether the result is an Ok result and satisfies a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test the Ok value.</param>
    /// <returns>True if the result is Ok and satisfies the predicate; otherwise, false.</returns>
    public abstract bool IsOkAnd(Func<T, bool> predicate);

    /// <summary>
    /// Determines whether the result is an Err result.
    /// </summary>
    /// <param name="value">The Err value if the result is Err; otherwise, the default value of E.</param>
    /// <returns>True if the result is Err; otherwise, false.</returns>
    public abstract bool IsErr([MaybeNullWhen(false)] out E value);

    /// <summary>
    /// Determines whether the result is an Err result and satisfies a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test the Err value.</param>
    /// <returns>True if the result is Err and satisfies the predicate; otherwise, false.</returns>
    public abstract bool IsErrAnd(Func<E, bool> predicate);

    /// <summary>
    /// Gets the Ok value if the result is Ok.
    /// </summary>
    /// <returns>The Ok value if the result is Ok; otherwise, null.</returns>
    public abstract T? Ok();

    /// <summary>
    /// Gets the Err value if the result is Err.
    /// </summary>
    /// <returns>The Err value if the result is Err; otherwise, null.</returns>
    public abstract E? Err();

    /// <summary>
    /// Maps an Ok result to another Ok result using a specified function.
    /// </summary>
    /// <typeparam name="U">The type of the new Ok value.</typeparam>
    /// <param name="op">The function to apply to the Ok value.</param>
    /// <returns>A new result with the mapped Ok value.</returns>
    public abstract Result<U, E> Map<U>(Func<T, U> op)
        where U : notnull;

    /// <summary>
    /// Maps an Ok result to another value or returns a default value if the result is Err.
    /// </summary>
    /// <typeparam name="U">The type of the new value.</typeparam>
    /// <param name="defaultValue">The default value to return if the result is Err.</param>
    /// <param name="op">The function to apply to the Ok value.</param>
    /// <returns>The mapped value or the default value.</returns>
    public abstract U MapOr<U>(U defaultValue, Func<T, U> op);

    /// <summary>
    /// Maps an Ok result to another value or applies a function to the Err value.
    /// </summary>
    /// <typeparam name="U">The type of the new value.</typeparam>
    /// <param name="f">The function to apply to the Err value.</param>
    /// <param name="op">The function to apply to the Ok value.</param>
    /// <returns>The mapped value.</returns>
    public abstract U MapOrElse<U>(Func<E, U> f, Func<T, U> op);

    /// <summary>
    /// Maps an Err result to another Err result using a specified function.
    /// </summary>
    /// <typeparam name="O">The type of the new Err value.</typeparam>
    /// <param name="op">The function to apply to the Err value.</param>
    /// <returns>A new result with the mapped Err value.</returns>
    public abstract Result<T, O> MapErr<O>(Func<E, O> op)
        where O : notnull;

    /// <summary>
    /// Applies a function to the Ok value and returns a new result.
    /// </summary>
    /// <typeparam name="U">The type of the new Ok value.</typeparam>
    /// <param name="op">The function to apply to the Ok value.</param>
    /// <returns>A new result with the mapped Ok value.</returns>
    public abstract Result<U, E> AndThen<U>(Func<T, Result<U, E>> op)
        where U : notnull;

    /// <summary>
    /// Applies a function to the Err value and returns a new result.
    /// </summary>
    /// <param name="op">The function to apply to the Err value.</param>
    /// <returns>A new result with the mapped Err value.</returns>
    public abstract Result<T, E> OrElse(Func<E, Result<T, E>> op);

    /// <summary>
    /// Unwraps the Ok value or throws an exception if the result is Err.
    /// </summary>
    /// <returns>The Ok value.</returns>
    /// <exception cref="UnwrapFailedException">Thrown if the result is Err.</exception>
    public abstract T Unwrap();

    /// <summary>
    /// Unwraps the Err value or throws an exception if the result is Ok.
    /// </summary>
    /// <returns>The Err value.</returns>
    /// <exception cref="UnwrapFailedException">Thrown if the result is Ok.</exception>
    public abstract E UnwrapErr();

    /// <summary>
    /// Unwraps the Ok value or returns the default value if the result is Err.
    /// </summary>
    /// <returns>The Ok value or the default value.</returns>
    public abstract T? UnwrapOrDefault();

    /// <summary>
    /// Unwraps the Ok value or returns a specified default value if the result is Err.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result is Err.</param>
    /// <returns>The Ok value or the default value.</returns>
    public abstract T UnwrapOr(T defaultValue);

    /// <summary>
    /// Unwraps the Ok value or applies a function to the Err value.
    /// </summary>
    /// <param name="f">The function to apply to the Err value.</param>
    /// <returns>The Ok value or the result of the function.</returns>
    public abstract T UnwrapOrElse(Func<E, T> f);

    /// <summary>
    /// Unwraps the Ok value or throws an exception with a specified message if the result is Err.
    /// </summary>
    /// <param name="message">The message to include in the exception.</param>
    /// <returns>The Ok value.</returns>
    /// <exception cref="UnwrapFailedException">Thrown if the result is Err.</exception>
    public abstract T Expect(string message);

    /// <summary>
    /// Unwraps the Err value or throws an exception with a specified message if the result is Ok.
    /// </summary>
    /// <param name="message">The message to include in the exception.</param>
    /// <returns>The Err value.</returns>
    /// <exception cref="UnwrapFailedException">Thrown if the result is Ok.</exception>
    public abstract E ExpectErr(string message);

    /// <summary>
    /// Returns an enumerable containing the Ok value if the result is Ok.
    /// </summary>
    /// <returns>An enumerable containing the Ok value or an empty enumerable.</returns>
    public abstract IEnumerable<T?> Iter();

    /// <summary>
    /// Creates an Ok result with a specified value.
    /// </summary>
    /// <param name="value">The Ok value.</param>
    /// <returns>A new Ok result.</returns>
    public static Result<T, E> Ok(T value) => new OkResult(value);

    /// <summary>
    /// Creates an Err result with a specified error.
    /// </summary>
    /// <param name="error">The Err value.</param>
    /// <returns>A new Err result.</returns>
    public static Result<T, E> Err(E error) => new ErrResult(error);

    /// <summary>
    /// Executes a function and wraps its result in a Result type.
    /// If the function executes successfully, it returns an Ok result with the function's return value.
    /// If an exception occurs during execution, it returns an Err result containing the exception.
    /// </summary>
    /// <typeparam name="U">The return type of the function and the Ok type of the Result.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>
    /// A Result object that is either:
    /// - Ok with the return value of the function if it executes successfully, or
    /// - Err with the caught exception if an exception occurs during execution, or
    /// - Err with ArgumentNullException if the function returned null.
    /// </returns>
    /// <remarks>
    /// This method provides a way to safely execute operations that might throw exceptions,
    /// converting them into the Result type for consistent error handling.
    /// </remarks>
    public static Result<U, Exception> Try<U>(Func<U> func)
        where U : notnull {
        try {
            return func() switch {
                null => Result<U, Exception>.Err(
                    new ArgumentNullException(nameof(func), NotNullMessage)
                ),
                U result => Result<U, Exception>.Ok(result),
            };
        } catch (Exception ex) {
            return Result<U, Exception>.Err(ex);
        }
    }

    private abstract class ResultBase<TValue>(TValue value) : Result<T, E>
    {
        protected TValue InternalValue { get; } = value;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) =>
        obj switch {
            ResultBase<TValue> other => Equals(InternalValue, other.InternalValue),
            _ => false,
        };

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() {
        return InternalValue?.GetHashCode() * 41 ?? 0;
    }

    public static bool operator ==(ResultBase<TValue>? left, ResultBase<TValue>? right) =>
        (left, right) switch {
            (ResultBase<TValue> leftResult, ResultBase<TValue> rightResult) => Equals(
                leftResult.InternalValue,
                rightResult.InternalValue
            ),
            _ => false,
        };

    public static bool operator !=(ResultBase<TValue>? left, ResultBase<TValue>? right) {
        return !(left == right);
    }
}

private sealed class OkResult(T value) : ResultBase<T>(value)
    {
        public override bool IsOk(out T value) {
    value = InternalValue;
    return true;
}

public override bool IsOkAnd(Func<T, bool> predicate) => predicate(InternalValue);

public override bool IsErr([MaybeNullWhen(false)] out E value) {
    value = default;
    return false;
}

public override bool IsErrAnd(Func<E, bool> predicate) => false;

public override T? Ok() => InternalValue;

public override E? Err() => default;

public override Result<U, E> Map<U>(Func<T, U> op) => Result<U, E>.Ok(op(InternalValue));

public override U MapOr<U>(U defaultValue, Func<T, U> op) => op(InternalValue);

public override U MapOrElse<U>(Func<E, U> f, Func<T, U> op) => op(InternalValue);

public override Result<T, O> MapErr<O>(Func<E, O> op) => Result<T, O>.Ok(InternalValue);

public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> op) => op(InternalValue);

public override Result<T, E> OrElse(Func<E, Result<T, E>> op) => this;

public override T Unwrap() => InternalValue;

public override E UnwrapErr() =>
    throw new UnwrapFailedException(
        $"Cannot unwrap Err from {nameof(OkResult)}",
        InternalValue
    );

public override T? UnwrapOrDefault() => InternalValue;

public override T UnwrapOr(T defaultValue) => InternalValue;

public override T UnwrapOrElse(Func<E, T> f) => InternalValue;

public override T Expect(string message) => InternalValue;

public override E ExpectErr(string message) =>
    throw new UnwrapFailedException(message, InternalValue);

public override IEnumerable<T?> Iter() => [InternalValue];
}

private sealed class ErrResult(E error) : ResultBase<E>(error)
    {
        public override bool IsOk([MaybeNullWhen(false)] out T value) {
    value = default;
    return false;
}

public override bool IsOkAnd(Func<T, bool> predicate) => false;

public override bool IsErr([MaybeNullWhen(false)] out E value) {
    value = InternalValue;
    return true;
}

public override bool IsErrAnd(Func<E, bool> predicate) => predicate(InternalValue);

public override T? Ok() => default;

public override E? Err() => InternalValue;

public override Result<U, E> Map<U>(Func<T, U> op) => Result<U, E>.Err(InternalValue);

public override U MapOr<U>(U defaultValue, Func<T, U> op) => defaultValue;

public override U MapOrElse<U>(Func<E, U> f, Func<T, U> op) => f(InternalValue);

public override Result<T, O> MapErr<O>(Func<E, O> op) =>
    Result<T, O>.Err(op(InternalValue));

public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> op) =>
    Result<U, E>.Err(InternalValue);

public override Result<T, E> OrElse(Func<E, Result<T, E>> op) => op(InternalValue);

public override T Unwrap() =>
    throw new UnwrapFailedException(
        $"Cannot unwrap Ok from {nameof(ErrResult)}",
        InternalValue
    );

public override E UnwrapErr() => InternalValue;

public override T? UnwrapOrDefault() => default;

public override T UnwrapOr(T defaultValue) => defaultValue;

public override T UnwrapOrElse(Func<E, T> f) => f(InternalValue);

public override T Expect(string message) =>
    throw new UnwrapFailedException(message, InternalValue);

public override E ExpectErr(string message) => InternalValue;

public override IEnumerable<T?> Iter() => [];
    }
}
