namespace Domain.UnitTests;

using Domain;

public class ResultTests
{
    private static Error GetTestError()
    {
        return new Error("Error code", "Error message");
    }

    private static Result<string> SqThenToString(int x)
    {
        try
        {
            checked
            {
                int sq = x * x;
                return Result<string>.Ok(sq.ToString());
            }
        }
        catch (OverflowException)
        {
            return Result<string>.Err(new Error("ErrorCode", "Overflowed"));
        }
    }

    [Fact]
    public void Result_WhenCreated_ShouldSetValuesOnOkOrErr()
    {
        var result = Result<int>.Ok(10);
        Assert.True(result.IsOk());

        result = Result<int>.Err(GetTestError());
        Assert.True(result.IsErr());
    }

    [Fact]
    public void IsOk_WhenCalled_ShouldReturnBooleanRepresentingOkOrErr()
    {
        var result = Result<int>.Ok(10);
        Assert.True(result.IsOk());

        result = Result<int>.Err(GetTestError());
        Assert.False(result.IsOk());
    }

    [Fact]
    public void IsOkAnd_WhenCalled_ShouldReturnBooleanRepresentingOkAndPredicate()
    {
        var result = Result<int>.Ok(10);
        Assert.True(result.IsOkAnd(value => value == 10));

        result = Result<int>.Err(GetTestError());
        Assert.False(result.IsOkAnd(value => value == 10));
    }

    [Fact]
    public void IsErr_WhenCalled_ShouldReturnBooleanRepresentingOkOrErr()
    {
        var result = Result<int>.Ok(10);
        Assert.False(result.IsErr());

        result = Result<int>.Err(GetTestError());
        Assert.True(result.IsErr());
    }

    [Fact]
    public void Unwrap_ShouldReturnValueOnOkOrThrowUnwrapExceptionOnErr()
    {
        var expectedValue = 100;
        var result = Result<int>.Ok(expectedValue);
        Assert.Equal(expectedValue, result.Unwrap());

        result = Result<int>.Err(GetTestError());
        Assert.Throws<Result<int>.UnwrapFailedException>(() => result.Unwrap());
    }

    [Fact]
    public void UnwrapOrDefault_WhenCalled_ShouldReturnValueOnOkOrDefaultOnErr()
    {
        var expectedValue = 100;
        var result = Result<int>.Ok(expectedValue);
        Assert.Equal(expectedValue, result.UnwrapOrDefault());

        result = Result<int>.Err(GetTestError());
        Assert.Equal(default(int), result.UnwrapOrDefault());
    }

    [Fact]
    public void Ok_WhenCalled_ReturnsValueOnOkOrNullOnErr()
    {
        var result = Result<string>.Ok("Hello World");
        Assert.Equal("Hello World", result.Ok());

        result = Result<string>.Err(GetTestError());
        Assert.Null(result.Ok());
    }

    [Fact]
    public void Err_WhenCalled_ReturnsErrorOnErrOrNullOnOk()
    {
        var result = Result<string>.Ok("Hello World");
        Assert.Null(result.Err());

        result = Result<string>.Err(GetTestError());
        Assert.Equal(GetTestError(), result.Err());
    }

    [Fact]
    public void Map_WhenCalled_ShouldMapAResultTToResultUByApplyingAFnLeavingAnErrUntouched()
    {
        var result = Result<int>.Ok(10);
        Assert.Equal(100, result.Map(x => x * x).Unwrap());

        result = Result<int>.Err(GetTestError());
        Assert.True(result.Map(x => x * x).IsErr());
    }

    [Fact]
    public void MapOr_WhenCalled_ReturnsTheProvidedDefaultOrMapsAFnToTheContainedOkValue()
    {
        var result = Result<string>.Ok("foo");
        Assert.Equal(3, result.MapOr(42, v => v.Length));

        result = Result<string>.Err(GetTestError());
        Assert.Equal(42, result.MapOr(42, v => v.Length));
    }

    [Fact]
    public void Iter_WhenCalled_ShouldIterateOverThePossibleContainedValue()
    {
        var result = Result<int>.Ok(7);
        Assert.Equal(7, result.Iter().First());

        result = Result<int>.Err(GetTestError());
        Assert.Empty(result.Iter().Take(1));
    }

    [Fact]
    public void Expect_ShouldThrowWithCustomMessageOnErr()
    {
        var result = Result<int>.Err(GetTestError());
        var customMessage = "Custom error message";
        var exception = Assert.Throws<Result<int>.UnwrapFailedException>(
            () => result.Expect(customMessage)
        );
        Assert.Contains(customMessage, exception.Message);
    }

    [Fact]
    public void AndThen_ShouldApplyFunctionOnOkAndLeavingAnErrUntouched()
    {
        var result = Result<int>.Ok(2).AndThen(SqThenToString);
        Assert.Equal("4", result.Unwrap());

        var overflowResult = Result<int>.Ok(1_000_000).AndThen(SqThenToString);
        Assert.Equal("Overflowed", overflowResult.Err().Description);

        var initialError = Result<int>
            .Err(new Error("ErrorCode", "Not a number"))
            .AndThen(SqThenToString);
        Assert.Equal("Not a number", initialError.Err().Description);
    }
}
