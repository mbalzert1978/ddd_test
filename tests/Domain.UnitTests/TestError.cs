namespace Domain.UnitTests;

using Domain;

public class ErrorTests
{
    [Fact]
    public void Error_WithEmptyCodeAndNonEmptyDescription_IsCreatedCorrectly()
    {
        // Arrange
        string code = string.Empty;
        string description = "Test Description";

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Error_WithEmptyCodeAndNullDescription_IsCreatedCorrectly()
    {
        // Arrange
        string code = string.Empty;
        string? description = null;

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Null(error.Description);
    }

    [Fact]
    public void Error_WithNonEmptyCodeAndNonEmptyDescription_IsCreatedCorrectly()
    {
        // Arrange
        string code = "Test Code";
        string description = "Test Description";

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Error_WithNonEmptyCodeAndNullDescription_IsCreatedCorrectly()
    {
        // Arrange
        string code = "Test Code";
        string? description = null;

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Null(error.Description);
    }

    [Fact]
    public void Error_WithNone_HasEmptyCodeAndDescription()
    {
        // Arrange
        string expectedCode = string.Empty;
        string expectedDescription = string.Empty;

        // Act
        Error error = Error.None;

        // Assert
        Assert.Equal(expectedCode, error.Code);
        Assert.Equal(expectedDescription, error.Description);
    }
}
