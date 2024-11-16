namespace Domain.UnitTests;

using Domain.Primitives;

public class ErrorTests {
    [Fact]
    public void Error_WithEmptyCodeAndNonEmptyDescription_IsCreatedCorrectly() {
        // Arrange
        var code = string.Empty;
        const string description = "Test Description";

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Error_WithEmptyCodeAndNullDescription_IsCreatedCorrectly() {
        // Arrange
        var code = string.Empty;
        string? description = null;

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Null(error.Description);
    }

    [Fact]
    public void Error_WithNonEmptyCodeAndNonEmptyDescription_IsCreatedCorrectly() {
        // Arrange
        const string code = "Test Code";
        const string description = "Test Description";

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Error_WithNonEmptyCodeAndNullDescription_IsCreatedCorrectly() {
        // Arrange
        const string code = "Test Code";
        string? description = null;

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Null(error.Description);
    }

    [Fact]
    public void Error_WithNone_HasEmptyCodeAndDescription() {
        // Arrange
        var expectedCode = string.Empty;
        var expectedDescription = string.Empty;

        // Act
        var error = Error.None;

        // Assert
        Assert.Equal(expectedCode, error.Code);
        Assert.Equal(expectedDescription, error.Description);
    }
}
