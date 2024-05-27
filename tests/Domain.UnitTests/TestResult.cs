namespace Domain.UnitTests;

using Domain;

public class ResultTests
{
    private static Error GetTestError()
    {
        return new Error("Error code", "Error message");
    }
    private static Result<double, string> Parse(string s)
    {
        try
        {
            var i = double.Parse(s);
            return Result<double, string>.Ok(i);
        }
        catch (FormatException)
        {
            return Result<double, string>.Err("Invalid format");
        }
        catch (OverflowException)
        {
            return Result<double, string>.Err("Number too large or too small");
        }
    }
    private static Result<int, int> Square(int x) => Result<int, int>.Ok(x * x);

    private static Result<int, int> Err(int x) => Result<int, int>.Err(x);
    private static int Count(string s) => s.Length;


    private static Result<string, string> SqThenToString(int x)
    {
        try
        {
            checked
            {
                int sq = x * x;
                return Result<string, string>.Ok(sq.ToString());
            }
        }
        catch (OverflowException)
        {
            return Result<string, string>.Err("Overflowed");
        }
    }

    [Fact]
    public void IsOk_WhenOkValueShouldReturnTrue()
    {
        Assert.True(Result<int, string>.Ok(10).IsOk());
        Assert.False(Result<int, string>.Err("Something went wrong").IsOk());
    }

    [Fact]
    public void IsOkAnd_WhenOkValueShouldMatchPredicate()
    {
        Assert.True(Result<int, string>.Ok(2).IsOkAnd(x => x > 1));
        Assert.False(Result<int, string>.Ok(0).IsOkAnd(x => x > 1));
        Assert.False(Result<int, string>.Err("Something went wrong").IsOkAnd(x => x > 1));
    }

    [Fact]
    public void IsErr_WhenErrValueShouldReturnTrue()
    {
        Assert.False(Result<int, string>.Ok(-3).IsErr());
        Assert.True(Result<int, string>.Err("Something went wrong").IsErr());
    }
    [Fact]
    public void IsErrAnd_WhenErrValueShouldMatchPredicate()
    {
        Assert.True(Result<int, Error>.Err(GetTestError()).IsErrAnd(x => x.GetType() == typeof(Error)));
        Assert.False(Result<int, Error>.Err(GetTestError()).IsErrAnd(x => x.GetType() == typeof(Exception)));
        Assert.False(Result<int, Error>.Ok(-3).IsErrAnd(x => x.GetType() == typeof(Error)));
    }
    [Fact]
    public void Ok_WhenOkValueShouldReturnValueOrDefault()
    {
        Assert.Equal(2, Result<int, string>.Ok(2).Ok());
        Assert.Equal(default, Result<int, string>.Err("Nothing here").Ok());
    }
    [Fact]
    public void Err_WhenErrValueShouldReturnValueOrDefault()
    {
        Assert.Equal(default, Result<int, string>.Ok(2).Err());
        Assert.Equal("Nothing here", Result<int, string>.Err("Nothing here").Err());
    }
    [Fact]
    public void Map_WhenResultShouldMapResultTEToResultUEbyApplyingAFunctionToAContainedOkValueLeavingAnErrUntouched()
    {
        Assert.Equal(Parse("5").Map(i => i * 2), Result<double, string>.Ok(10));
        Assert.Equal(Parse("Nothing here").Map(i => i * 2), Result<double, string>.Err("Invalid format"));
    }
    [Fact]
    public void MapOr_WhenResultShouldApplyAFunctionToContainedValueOrDefault()
    {
        Assert.Equal(3, Result<string, string>.Ok("foo").MapOr(42, v => v.Length));
        Assert.Equal(42, Result<string, string>.Err("bar").MapOr(42, v => v.Length));
    }
    [Fact]
    public void MapOrElse_WhenResultShouldApplyAFunctionToContainedValueOrApplyFallbackFunction()
    {
        int k = 21;
        Assert.Equal(3, Result<string, string>.Ok("foo").MapOrElse(e => k * 2, v => v.Length));
        Assert.Equal(42, Result<string, string>.Err("bar").MapOrElse(e => k * 2, v => v.Length));
    }
    [Fact]
    public void MapErr_WhenResultShouldMapResultTEToResultTFbyApplyingAFunctionToAContainedErrValueLeavingAnOkUntouched()
    {
        Assert.Equal(Result<int, int>.Ok(5).MapErr(i => i.ToString()), Result<int, string>.Ok(5));
        Assert.Equal(Result<int, int>.Err(10).MapErr(i => i.ToString()), Result<int, string>.Err("10"));
    }
    [Fact]
    public void Iter_WhenOkValueShouldYieldValueOrDefault()
    {
        Assert.Equal(5, Result<int, string>.Ok(5).Iter().First());
        Assert.Empty(Result<int, string>.Err("Nothing here").Iter());
    }
    [Fact]
    public void Expect_WhenOkValueShouldReturnTheValueOrThrowsAnErrorWithTheGivenMessage()
    {
        Assert.Equal("Something went wrong", Assert.Throws<Result<int, string>.UnwrapFailedException>(() => Result<int, string>.Err("Emergency failure").Expect("Something went wrong")).Message);
    }
    [Fact]
    public void Unwrap_WhenOkValueShouldReturnsTheValueOrThrowsAnUnwrapFailedException()
    {
        Assert.Equal("Cannot unwrap a failure result.", Assert.Throws<Result<int, string>.UnwrapFailedException>(() => Result<int, string>.Err("Emergency failure").Unwrap()).Message);
    }
    [Fact]
    public void UnwrapOrDefault_WhenOkValueShouldReturnTheValueOrDefault()
    {
        Assert.Equal(1909, Parse("1909").UnwrapOrDefault());
        Assert.Equal(default, Parse("1900blarg").UnwrapOrDefault());
    }
    [Fact]
    public void ExpectErr_WhenErrValueShouldReturnErrorOrThrowsAnErrorWithTheGivenMessage()
    {
        Assert.Equal("Testing expect error", Assert.Throws<Result<int, string>.UnwrapFailedException>(() => Result<int, string>.Ok(10).ExpectErr("Testing expect error")).Message);
    }
    [Fact]
    public void UnwrapErr_WhenErrValueShouldReturnErrorOrThrowsAnUnwrapFailedException()
    {
        Assert.Equal("Cannot unwrap error from a success result.", Assert.Throws<Result<int, string>.UnwrapFailedException>(() => Result<int, string>.Ok(10).UnwrapErr()).Message);
    }
    [Fact]
    public void AndThen_WhenOkvalueShouldCallTheGivenFunctionOrReturnTheErrorValue()
    {
        Assert.Equal(Result<int, string>.Ok(2).AndThen(SqThenToString), Result<string, string>.Ok("4"));
        Assert.Equal(Result<int, string>.Ok(1_000_000).AndThen(SqThenToString), Result<string, string>.Err("Overflowed"));
        Assert.Equal(Result<int, string>.Err("Not a number").AndThen(SqThenToString), Result<string, string>.Err("Not a number"));
    }
    [Fact]
    public void OrElse_WhenErrValueShouldCallTheGivenFunctionOrReturnTheOkValue()
    {
        Assert.Equal(Result<int, int>.Ok(2).OrElse(Square).OrElse(Square), Result<int, int>.Ok(2));
        Assert.Equal(Result<int, int>.Ok(2).OrElse(Err).OrElse(Square), Result<int, int>.Ok(2));
        Assert.Equal(Result<int, int>.Err(3).OrElse(Square).OrElse(Err), Result<int, int>.Ok(9));
        Assert.Equal(Result<int, int>.Err(3).OrElse(Err).OrElse(Err), Result<int, int>.Err(3));
    }
    [Fact]
    public void UnwrapOr_WhenOkValueShouldReturnValueOrProvidedDefault()
    {
        int defaultValue = 42;
        Assert.Equal(2, Result<int, string>.Ok(2).UnwrapOr(defaultValue));
        Assert.Equal(42, Result<int, string>.Err("Something went wrong").UnwrapOr(defaultValue));
    }
    [Fact]
    public void UnwrapOrElse_WhenOkValueShouldReturnValueOrComputeFromFunction()
    {
        Assert.Equal(2, Result<int, string>.Ok(2).UnwrapOrElse(Count));
        Assert.Equal(3, Result<int, string>.Err("foo").UnwrapOrElse(Count));
    }

}
