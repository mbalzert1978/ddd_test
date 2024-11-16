using Domain.Primitives;

namespace Domain.UnitTests;

public class EntityTests {
    [Fact]
    public void Entity_WhenComparedToAnotherEntityShouldReturnFalseWhenTheyAreNotTheSameType() {
        var (a, b) = Setup();

        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingEqualsMethod_ReturnFalse() {
        var (a, b) = Setup();

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingIEquatableInterface_ReturnFalse() {
        var (a, b) = Setup();

        Assert.False(((IEquatable<Entity>)a).Equals(b));
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingObjectReferenceEquals_ReturnFalse() {
        var (a, b) = Setup();

        Assert.False(ReferenceEquals(a, b));
    }

    [Fact]
    public void EntitiesOfDifferentTypes_WhenComparedUsingObjectReferenceEquals_WithNull_ReturnFalse() {
        // Arrange
        FakeEntityA a = new(Guid.NewGuid());
        Entity? b = null;

        Assert.False(ReferenceEquals(a, b));
    }

    private class FakeEntityA(Guid id) : Entity(id) { }

    private class FakeEntityB(Guid id) : Entity(id) { }

    private static (FakeEntityA, FakeEntityB) Setup() {
        FakeEntityA a = new(Guid.NewGuid());
        FakeEntityB b = new(Guid.NewGuid());
        return (a, b);
    }
}
