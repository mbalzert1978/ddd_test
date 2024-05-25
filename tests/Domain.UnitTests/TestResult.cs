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
                return Result<string>.Success(sq.ToString());
            }
        }
        catch (OverflowException)
        {
            return Result<string>.Failure(new Error("ErrorCode", "overflowed"));
        }
    }

    [Fact]
    public void Result_when_created_should_set_values_on_success_or_failure()
    {
        var result = Result<int>.Success(10);
        Assert.True(result.IsSuccess);

        result = Result<int>.Failure(GetTestError());
        Assert.True(result.IsFailure);
    }



    [Fact]
    public void IsOk_when_called_should_return_boolean_representing_success_or_failure()
    {
        var result = Result<int>.Success(10);
        Assert.True(result.Is_Ok());

        result = Result<int>.Failure(GetTestError());
        Assert.False(result.Is_Ok());
    }

    [Fact]
    public void IsOkAnd_when_called_should_return_boolean_representing_success_and_predicate()
    {
        var result = Result<int>.Success(10);
        Assert.True(result.IsOkAnd(value => value == 10));

        result = Result<int>.Failure(GetTestError());
        Assert.False(result.IsOkAnd(value => value == 10));
    }
    [Fact]
    public void IsErr_when_called_should_return_boolean_representing_success_or_failure()
    {
        var result = Result<int>.Success(10);
        Assert.False(result.Is_Err());

        result = Result<int>.Failure(GetTestError());
        Assert.True(result.Is_Err());
    }

    [Fact]
    public void Unwrap_Should_Return_Value_On_Success_or_throw_unwrap_exception_on_failure()
    {
        var expectedValue = 100;
        var result = Result<int>.Success(expectedValue);
        Assert.Equal(expectedValue, result.Unwrap());

        result = Result<int>.Failure(GetTestError());
        Assert.Throws<Result<int>.UnwrapFailedException>(() => result.Unwrap());
    }
    [Fact]
    public void UnwrapOrDefault_when_called_Should_Return_Value_On_Success_or_default_on_failure()
    {
        var expectedValue = 100;
        var result = Result<int>.Success(expectedValue);
        Assert.Equal(expectedValue, result.UnwrapOrDefault());

        result = Result<int>.Failure(GetTestError());
        Assert.Equal(default(int), result.UnwrapOrDefault());
    }

    [Fact]
    public void Ok_when_called_returns_value_on_success_or_null_on_failure()
    {
        var result = Result<string>.Success("Hello World");
        Assert.Equal("Hello World", result.Ok());

        result = Result<string>.Failure(GetTestError());
        Assert.Null(result.Ok());
    }
    [Fact]
    public void Err_when_called_returns_error_on_failure_or_null_on_success()
    {
        var result = Result<string>.Success("Hello World");
        Assert.Null(result.Err());

        result = Result<string>.Failure(GetTestError());
        Assert.Equal(GetTestError(), result.Err());
    }

    [Fact]
    public void Map_when_called_should_map_a_result_t_to_result_u_by_applying_a_fn_leaving_an_failure_untouched()
    {
        var result = Result<int>.Success(10).Map(x => x * x);
        Assert.Equal(100, result.Unwrap());

        result = Result<int>.Failure(GetTestError()).Map(x => x * x);
        Assert.True(result.Is_Err());
    }
    [Fact]
    public void MapOr_when_called_returns_the_provided_default_or_maps_a_fn_to_the_contained_success_value()
    {
        var result = Result<string>.Success("foo");
        Assert.Equal(3, result.MapOr(42, v => v.Length));

        result = Result<string>.Failure(GetTestError());
        Assert.Equal(42, result.MapOr(42, v => v.Length));

    }
    [Fact]
    public void Iter_when_called_should_iterate_over_the_possible_contained_value()
    {
        var result = Result<int>.Success(7);
        Assert.Equal(7, result.Iter().First());

        result = Result<int>.Failure(GetTestError());
        Assert.Empty(result.Iter().Take(1));
    }

    [Fact]
    public void Expect_Should_Throw_With_Custom_Message_On_Failure()
    {
        var result = Result<int>.Failure(GetTestError());
        var customMessage = "Custom failure message";
        var exception = Assert.Throws<Result<int>.UnwrapFailedException>(() => result.Expect(customMessage));
        Assert.Contains(customMessage, exception.Message);
    }

    [Fact]
    public void AndThen_Should_Apply_Function_On_Success()
    {
        var result = Result<int>.Success(2).AndThen(SqThenToString);
        Assert.Equal("4", result.Unwrap());

        var overflowResult = Result<int>.Success(1_000_000).AndThen(SqThenToString);
        Assert.Equal("overflowed", overflowResult.Err().Description);

        var initialError = Result<int>.Failure(new Error("ErrorCode", "not a number")).AndThen(SqThenToString);
        Assert.Equal("not a number", initialError.Err().Description);
    }

}
