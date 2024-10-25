using Domain.Primitives;
using FluentAssertions;

namespace Domain.UnitTests;

public class ResultTests {
    private static string GetTestError() => "Error.Test";

    private static Result<int, int> Square(int x) => Result<int, int>.Ok(x * x);

    private static Result<int, int> Err(int x) => Result<int, int>.Err(x);

    private static int Count(string s) => s.Length;

    private static Result<string, string> SquareThenToString(int x) =>
        Result<int, string>
            .Try(() => {
                checked {
                    return x * x;
                }
            })
            .Map(sq => sq.ToString())
            .MapErr(_ => "Overflowed");

    [Theory]
    [InlineData(10)]
    public void Result_WhenOk_ShouldBeOk(int value) {
        Result<int, string>.Ok(value).IsOk(out var v).Should().BeTrue();
        v.Should().BeOfType(typeof(int)).And.Be(value);
    }

    [Fact]
    public void Result_WhenErr_ShouldNotBeOk() {
        Result<int, string>.Err("Something went wrong").IsOk(out _).Should().BeFalse();
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(0, false)]
    public void Result_WhenOkAndCheckedAgainstPredicate_ShouldMatchPredicate(
        int value,
        bool expectedResult
    ) {
        Result<int, string>.Ok(value).IsOkAnd(x => x > 1).Should().Be(expectedResult);
    }

    [Fact]
    public void Result_WhenErrAndCheckedAgainstPredicate_ShouldBeFalse() {
        Result<int, string>.Err("Something went wrong").IsOkAnd(x => x > 1).Should().BeFalse();
    }

    [Fact]
    public void Result_WhenOk_ShouldNotBeErr() {
        Result<int, string>.Ok(-3).IsErr(out _).Should().BeFalse();
    }

    [Fact]
    public void Result_WhenErr_ShouldBeErr() {
        Result<int, string>.Err("Something went wrong").IsErr(out _).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Result_WhenErrAndCheckedAgainstPredicate_ShouldMatchPredicate(bool expectedResult) {
        Result<int, string>
            .Err(GetTestError())
            .IsErrAnd(x =>
                expectedResult ? x.GetType() == typeof(string) : x.GetType() == typeof(Exception)
            )
            .Should()
            .Be(expectedResult);
    }

    [Fact]
    public void Result_WhenOkAndCheckedForErr_ShouldBeFalse() {
        Result<int, string>.Ok(-3).IsErrAnd(x => x.GetType() == typeof(string)).Should().BeFalse();
    }

    [Theory]
    [InlineData(2)]
    public void Result_WhenOk_ShouldReturnValue(int value) {
        Result<int, string>.Ok(value).Ok().Should().Be(value);
    }

    [Fact]
    public void Result_WhenErrAndAccessingOk_ShouldReturnDefault() {
        Result<int, string>.Err("Nothing here").Ok().Should().Be(default);
    }

    [Theory]
    [InlineData(2)]
    public void Result_WhenOkAndAccessingErr_ShouldReturnDefault(int value) {
        Result<int, string>.Ok(value).Err().Should().Be(default);
    }

    [Fact]
    public void Result_WhenErr_ShouldReturnErrValue() {
        const string errorMessage = "Nothing here";
        Result<int, string>.Err(errorMessage).Err().Should().Be(errorMessage);
    }

    [Theory]
    [InlineData("5", 10)]
    public void Result_WhenOkAndMapped_ShouldApplyFunctionToValue(string input, double expected) {
        Result<int, Exception>
            .Try(() => double.Parse(input))
            .Map(i => i * 2)
            .Should()
            .Be(Result<double, Exception>.Ok(expected));
    }

    [Fact]
    public void Result_WhenErrAndMapped_ShouldLeaveErrUntouched() {
        var result = Result<int, Exception>.Try(() => double.Parse("Nothing here")).Map(i => i * 2);

        result.IsErr(out var error).Should().Be(true);
        error
            .Should()
            .BeOfType<FormatException>()
            .Which.Message.Should()
            .Be("The input string 'Nothing here' was not in a correct format.");
    }

    [Theory]
    [InlineData("foo", 3)]
    public void Result_WhenOkAndMapOr_ShouldApplyFunctionToValue(string input, int expected) {
        Result<string, string>.Ok(input).MapOr(42, v => v.Length).Should().Be(expected);
    }

    [Fact]
    public void Result_WhenErrAndMapOr_ShouldReturnDefault() {
        Result<string, string>.Err("bar").MapOr(42, v => v.Length).Should().Be(42);
    }

    [Theory]
    [InlineData("foo", 3)]
    public void Result_WhenOkAndMapOrElse_ShouldApplyFunctionToValue(string input, int expected) {
        int k = 21;
        Result<string, string>.Ok(input).MapOrElse(e => k * 2, v => v.Length).Should().Be(expected);
    }

    [Fact]
    public void Result_WhenErrAndMapOrElse_ShouldApplyFallbackFunction() {
        int k = 21;
        Result<string, string>.Err("bar").MapOrElse(e => k * 2, v => v.Length).Should().Be(42);
    }

    [Theory]
    [InlineData(5)]
    public void Result_WhenOkAndMapErr_ShouldLeaveOkUntouched(int value) {
        Result<int, int>
            .Ok(value)
            .MapErr(i => i.ToString())
            .Should()
            .Be(Result<int, string>.Ok(value));
    }

    [Theory]
    [InlineData(10, "10")]
    public void Result_WhenErrAndMapErr_ShouldApplyFunctionToErrValue(int input, string expected) {
        Result<int, int>
            .Err(input)
            .MapErr(i => i.ToString())
            .Should()
            .Be(Result<int, string>.Err(expected));
    }

    [Theory]
    [InlineData(5)]
    public void Result_WhenOkAndIterated_ShouldYieldValue(int value) {
        Result<int, string>.Ok(value).Iter().Should().ContainSingle().And.Contain(value);
    }

    [Fact]
    public void Result_WhenErrAndIterated_ShouldYieldEmpty() {
        Result<int, string>.Err("Nothing here").Iter().Should().BeEmpty();
    }

    [Fact]
    public void Result_WhenErrAndExpected_ShouldThrowWithMessage() {
        const string expectedMessage = "Something went wrong";
        Action act = () => Result<int, string>.Err("Emergency failure").Expect(expectedMessage);
        act.Should()
            .Throw<Result<int, string>.UnwrapFailedException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public void Result_WhenErrAndUnwrapped_ShouldThrowUnwrapFailedException() {
        Action act = () => Result<int, string>.Err("Emergency failure").Unwrap();
        act.Should()
            .Throw<Result<int, string>.UnwrapFailedException>()
            .WithMessage("Cannot unwrap Ok from ErrResult");
    }

    [Theory]
    [InlineData("1909", 1909)]
    public void Result_WhenOkAndUnwrappedOrDefault_ShouldReturnValue(string input, double expected) {
        Result<int, Exception>
            .Try(() => double.Parse(input))
            .UnwrapOrDefault()
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Result_WhenErrAndUnwrappedOrDefault_ShouldReturnDefault() {
        Result<int, Exception>
            .Try(() => double.Parse("1900foo"))
            .UnwrapOrDefault()
            .Should()
            .Be(default);
        // Parse("1900foo").UnwrapOrDefault().Should().Be(default);
    }

    [Theory]
    [InlineData(10)]
    public void Result_WhenOkAndExpectErr_ShouldThrowWithMessage(int value) {
        const string expectedMessage = "Testing expect error";
        Action act = () => Result<int, string>.Ok(value).ExpectErr(expectedMessage);
        act.Should()
            .Throw<Result<int, string>.UnwrapFailedException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData(10)]
    public void Result_WhenOkAndUnwrapErr_ShouldThrowUnwrapFailedException(int value) {
        Action act = () => Result<int, string>.Ok(value).UnwrapErr();
        act.Should()
            .Throw<Result<int, string>.UnwrapFailedException>()
            .WithMessage("Cannot unwrap Err from OkResult");
    }

    [Theory]
    [InlineData(2, "4")]
    public void Result_WhenOkAndAndThen_ShouldCallFunction(int input, string expected) {
        Result<int, string>
            .Ok(input)
            .AndThen(SquareThenToString)
            .Should()
            .Be(Result<string, string>.Ok(expected));
    }

    [Fact]
    public void Result_WhenOkAndAndThenOverflows_ShouldReturnErr() {
        Result<int, string>
            .Ok(1_000_000)
            .AndThen(SquareThenToString)
            .Should()
            .Be(Result<string, string>.Err("Overflowed"));
    }

    [Fact]
    public void Result_WhenErrAndAndThen_ShouldReturnErr() {
        const string errorMessage = "Not a number";
        Result<int, string>
            .Err(errorMessage)
            .AndThen(SquareThenToString)
            .Should()
            .Be(Result<string, string>.Err(errorMessage));
    }

    [Theory]
    [InlineData(2)]
    public void Result_WhenOkAndOrElse_ShouldReturnOkValue(int value) {
        Result<int, int>
            .Ok(value)
            .OrElse(Square)
            .OrElse(Square)
            .Should()
            .Be(Result<int, int>.Ok(value));
    }

    [Theory]
    [InlineData(2)]
    public void Result_WhenOkAndOrElseAfterErr_ShouldReturnOkValue(int value) {
        Result<int, int>
            .Ok(value)
            .OrElse(Err)
            .OrElse(Square)
            .Should()
            .Be(Result<int, int>.Ok(value));
    }

    [Theory]
    [InlineData(3, 9)]
    public void Result_WhenErrAndOrElse_ShouldCallFunction(int input, int expected) {
        Result<int, int>
            .Err(input)
            .OrElse(Square)
            .OrElse(Err)
            .Should()
            .Be(Result<int, int>.Ok(expected));
    }

    [Theory]
    [InlineData(3)]
    public void Result_WhenErrAndOrElseReturnsErr_ShouldReturnErr(int value) {
        Result<int, int>
            .Err(value)
            .OrElse(Err)
            .OrElse(Err)
            .Should()
            .Be(Result<int, int>.Err(value));
    }

    [Theory]
    [InlineData(2)]
    public void Result_WhenOkAndUnwrapOr_ShouldReturnValue(int value) {
        Result<int, string>.Ok(value).UnwrapOr(42).Should().Be(value);
    }

    [Fact]
    public void Result_WhenErrAndUnwrapOr_ShouldReturnDefault() {
        Result<int, string>.Err("Something went wrong").UnwrapOr(42).Should().Be(42);
    }

    [Theory]
    [InlineData(2)]
    public void Result_WhenOkAndUnwrapOrElse_ShouldReturnValue(int value) {
        Result<int, string>.Ok(value).UnwrapOrElse(Count).Should().Be(value);
    }

    [Fact]
    public void Result_WhenErrAndUnwrapOrElse_ShouldComputeFromFunction() {
        Result<int, string>.Err("foo").UnwrapOrElse(Count).Should().Be(3);
    }

    [Fact]
    public void Result_Try_WhenFunctionSucceeds_ShouldReturnOk() {
        var result = Result<int, Exception>.Try(() => 42);

        result.IsOk(out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void Result_Try_WhenFunctionThrows_ShouldReturnErr() {
        var result = Result<int, Exception>.Try<int>(
            () => throw new InvalidOperationException("Test exception")
        );

        result.IsErr(out var error).Should().BeTrue();
        error
            .Should()
            .BeOfType<InvalidOperationException>()
            .Which.Message.Should()
            .Be("Test exception");
    }

    [Fact]
    public void Result_Try_WhenFunctionReturnsNull_ShouldThrowArgumentNullException() {
        var result = Result<string, Exception>.Try<string>(() => null!);

        result.IsErr(out var error).Should().BeTrue();
        error
            .Should()
            .BeOfType<ArgumentNullException>()
            .Which.Message.Should()
            .Be(
                "The function provided to Result.Try returned null, which violates the non-null constraint. (Parameter 'func')"
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Result_Try_WhenFunctionDependsOnInput_ShouldWorkCorrectly(int input) {
        var result = Result<int, Exception>.Try(() => input * 2);

        result.IsOk(out var value).Should().BeTrue();
        value.Should().Be(input * 2);
    }
}
