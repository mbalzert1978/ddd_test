using Domain.Primitives;
using FluentAssertions;
using static Domain.Primitives.Factories;

namespace Domain.UnitTests;

public class ResultTests {
    private const string ErrorMessage = "Nothing here";

    private static Result<int, int> Square(int x) => Ok<int, int>(x * x);

    private static Result<int, int> Err(int x) => Err<int, int>(x);

    private static Result<string, string> SquareThenToString(int x) =>
        Try(() => checked(x * x)).Map(sq => sq.ToString()).MapErr(_ => "Overflowed");

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-5)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Result_WhenCreatedWithOkValue_ShouldReturnTrueForIsOkAndContainCorrectValue(
        int value
    ) {
        Ok(value).IsOk(out var v).Should().BeTrue();
        v.Should().BeOfType(typeof(int)).And.Be(value);
    }

    [Fact]
    public void Result_WhenCreatedWithErrValue_ShouldReturnFalseForIsOk() =>
        Err<int>(ErrorMessage).IsOk().Should().BeFalse();

    [Theory]
    [InlineData(2, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(int.MaxValue, true)]
    public void Result_WhenOkAndCheckedAgainstPredicate_ShouldReturnExpectedResult(
        int value,
        bool expectedResult
    ) => Ok(value).IsOk(x => x > 1).Should().Be(expectedResult);

    [Fact]
    public void Result_WhenErrAndCheckedAgainstPredicate_ShouldReturnFalse() =>
        Err<int>(ErrorMessage).IsOk(x => x > 1).Should().BeFalse();

    [Theory]
    [InlineData(-3)]
    [InlineData(0)]
    [InlineData(42)]
    public void Result_WhenCreatedWithOkValue_ShouldReturnFalseForIsErr(int value) =>
        Ok(value).IsErr().Should().BeFalse();

    [Fact]
    public void Result_WhenCreatedWithErrValue_ShouldReturnTrueForIsErr() =>
        Err<int>(ErrorMessage).IsErr().Should().BeTrue();

    [Fact]
    public void Result_WhenCreatedWithErrValue_ShouldReturnTrueForIsErrAndContainCorrectValue() {
        Err<int>(ErrorMessage).IsErr(out var v).Should().BeTrue();
        v.Should().Be(ErrorMessage);

    }

    [Theory]
    [InlineData("Short")]
    [InlineData("This is a long error message")]
    public void Result_WhenErrAndCheckedAgainstErrPredicate_ShouldReturnExpectedResult(
        string errorMessage
    ) => Err<int>(errorMessage).IsErr(err => err.Length > 10).Should().Be(errorMessage.Length > 10);

    [Fact]
    public void Result_WhenOkAndCheckedAgainstErrPredicate_ShouldReturnFalse() =>
        Ok(-3).IsErr(err => err.Length > 10).Should().BeFalse();

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Result_WhenOk_ShouldReturnCorrectValueForOptionalOkAndDefaultForOptionalErr(
        int value
    ) {
        var result = Ok(value);
        result.OptionalOk.Should().Be(value);
        result.OptionalErr.Should().Be(default);
    }

    [Fact]
    public void Result_WhenErr_ShouldReturnDefaultForOptionalOkAndCorrectValueForOptionalErr() {
        var result = Err<int>(ErrorMessage);
        result.OptionalOk.Should().Be(default);
        result.OptionalErr.Should().Be(ErrorMessage);
    }

    [Theory]
    [InlineData("5", 10)]
    [InlineData("0", 0)]
    [InlineData("-3", -6)]
    public void Result_WhenOkAndMapped_ShouldApplyFunctionToValueAndReturnNewOk(
        string input,
        double expected
    ) =>
        Try(() => double.Parse(input)).Map(i => i * 2).Should().Be(Ok<double, Exception>(expected));

    [Fact]
    public void Result_WhenErrAndMapped_ShouldReturnOriginalErr() =>
        Try(() => double.Parse("Nothing here"))
            .Map(i => i * 2)
            .IsErr(error => error is FormatException);

    [Theory]
    [InlineData("foo", 3)]
    [InlineData("", 0)]
    [InlineData("hello world", 11)]
    public void Result_WhenOkAndMapOr_ShouldApplyFunctionToValue(string input, int expected) =>
        Ok(input).MapOr(42, v => v.Length).Should().Be(expected);

    [Fact]
    public void Result_WhenErrAndMapOr_ShouldReturnDefaultValue() =>
        Err<string>("bar").MapOr(42, v => v.Length).Should().Be(42);

    [Theory]
    [InlineData("foo", 3)]
    [InlineData("", 0)]
    [InlineData("hello world", 11)]
    public void Result_WhenOkAndMapOrElse_ShouldApplyFunctionToValue(string input, int expected) =>
        Ok(input).MapOrElse(e => 21 * 2, v => v.Length).Should().Be(expected);

    [Fact]
    public void Result_WhenErrAndMapOrElse_ShouldApplyFallbackFunction() =>
        Err<string>("bar").MapOrElse(e => 21 * 2, v => v.Length).Should().Be(42);

    [Theory]
    [InlineData(5)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Result_WhenOkAndMapErr_ShouldReturnOriginalOk(int value) =>
        Ok<int, int>(value).MapErr(i => i.ToString()).Should().Be(Ok(value));

    [Theory]
    [InlineData(10, "10")]
    [InlineData(0, "0")]
    [InlineData(-5, "-5")]
    public void Result_WhenErrAndMapErr_ShouldApplyFunctionToErrValue(int input, string expected) =>
        Err<int, int>(input).MapErr(i => i.ToString()).Should().Be(Err<int>(expected));

    [Fact]
    public void Result_WhenErrAndExpect_ShouldThrowUnwrapFailedExceptionWithSpecifiedMessage() =>
        Assert.Throws<UnwrapFailedException>(
            () => Err<int>(ErrorMessage).Expect("Testing expect")
        );

    [Theory]
    [InlineData("1909", 1909)]
    [InlineData("0", 0)]
    [InlineData("-42", -42)]
    public void Result_WhenOkAndOr_ShouldReturnOriginalValue(string input, double expected) =>
        Try(() => double.Parse(input)).Or(12.0).Should().Be(expected);

    [Fact]
    public void Result_WhenErrAndOr_ShouldReturnDefaultValue() =>
        Try(() => double.Parse("1900foo")).Or(12.0).Should().Be(12.0);

    [Theory]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-5)]
    public void Result_WhenOkAndExpectErr_ShouldThrowUnwrapFailedExceptionWithSpecifiedMessage(
        int value
    ) => Assert.Throws<UnwrapFailedException>(() => Ok(value).ExpectErr("Testing expectErr"));

    [Theory]
    [InlineData(2, "4")]
    [InlineData(0, "0")]
    [InlineData(-3, "9")]
    public void Result_WhenOkAndAndThen_ShouldApplyFunctionAndReturnNewResult(
        int input,
        string expected
    ) => Ok(input).AndThen(SquareThenToString).Should().Be(Ok(expected));

    [Fact]
    public void Result_WhenOkAndAndThenOverflows_ShouldReturnErrResult() =>
        Ok(1_000_000).AndThen(SquareThenToString).Should().Be(Err<string>("Overflowed"));

    [Fact]
    public void Result_WhenErrAndAndThen_ShouldReturnOriginalErr() =>
        Err<int>("Not a number")
            .AndThen(SquareThenToString)
            .Should()
            .Be(Err<string>("Not a number"));

    [Theory]
    [InlineData(2)]
    [InlineData(0)]
    [InlineData(-5)]
    public void Result_WhenOkAndOrElse_ShouldReturnOriginalOk(int value) =>
        Ok<int, int>(value).OrElse(Square).OrElse(Square).Should().Be(Ok<int, int>(value));

    [Theory]
    [InlineData(3, 9)]
    [InlineData(0, 0)]
    [InlineData(-2, 4)]
    public void Result_WhenErrAndOrElse_ShouldApplyFunctionAndReturnNewResult(
        int input,
        int expected
    ) => Err(input).OrElse(Square).OrElse(Err).Should().Be(Ok<int, int>(expected));

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Result_WhenErrAndOrElseReturnsErr_ShouldReturnFinalErr(int value) =>
        Err(value).OrElse(Err).OrElse(Err).Should().Be(Err<int, int>(value));

    [Fact]
    public void Result_WhenTrySucceeds_ShouldReturnOkWithCorrectValue() =>
        Try(() => 42).IsOk(v => v == 42).Should().BeTrue();

    [Fact]
    public void Result_WhenTryThrows_ShouldReturnErrWithCorrectException() =>
        Try<int>(() => throw new InvalidOperationException("Test exception"))
            .IsErr(error => {
                Assert.IsType<InvalidOperationException>(error);
                return error.Message == "Test exception";
            })
            .Should()
            .BeTrue();

    [Fact]
    public void Result_WhenTryReturnsNull_ShouldReturnErrWithArgumentNullException() =>
        Try<string>(() => null!)
            .IsErr(error => {
                Assert.IsType<ArgumentNullException>(error);
                return error.Message == "Value cannot be null. (Parameter 'value')";
            })
            .Should()
            .BeTrue();

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Result_WhenTryWithInput_ShouldReturnCorrectResult(int input) =>
        Try(() => input * 2).IsOk(v => v == input * 2).Should().BeTrue();
}
