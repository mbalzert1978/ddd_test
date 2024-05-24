namespace Domain.UnitTests;

public class EntityTests
{
    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingEqualityOperator_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity1(Guid.NewGuid());
        var entity2 = new TestEntity2(Guid.NewGuid());

        // Act
        var result1 = entity1 == entity2;
        var result2 = entity1!= entity2;

        // Assert
        Assert.False(result1);
        Assert.True(result2);
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingEqualsMethod_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity1(Guid.NewGuid());
        var entity2 = new TestEntity2(Guid.NewGuid());

        // Act
        var result1 = entity1.Equals(entity2);
        var result2 = entity1.Equals((object)entity2);

        // Assert
        Assert.False(result1);
        Assert.False(result2);
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingIEquatableInterface_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity1(Guid.NewGuid());
        var entity2 = new TestEntity2(Guid.NewGuid());

        // Act
        var result = ((IEquatable<Entity>)entity1).Equals(entity2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingObjectReferenceEquals_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity1(Guid.NewGuid());
        var entity2 = new TestEntity2(Guid.NewGuid());

        // Act
        var result = ReferenceEquals(entity1, entity2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingObjectReferenceEquals_WithNull_ReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity1(Guid.NewGuid());
        Entity? entity2 = null;

        // Act
        var result = ReferenceEquals(entity1, entity2);

        // Assert
        Assert.False(result);
    }

    private class TestEntity1 : Entity
    {
        public TestEntity1(Guid id) : base(id) { }
    }

    private class TestEntity2 : Entity
    {
        public TestEntity2(Guid id) : base(id) { }
    }
}
