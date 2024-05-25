namespace Domain.UnitTests;

using Domain;

public class ResultTests
{
    [Fact]
    public void Success_Should_Create_Successful_Result()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_When_called_with_Error_Should_Create_Failure_Result_With_Error()
    {
        // Arrange
        var error = new Error("TestError", "Test error description");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_For_Invalid_Failure_Error_Combination()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));
        Assert.Equal("Result must be either success or failure, but not both. (Parameter 'error')",
                     exception.Message);
    }
}
