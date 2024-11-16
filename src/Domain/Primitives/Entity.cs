namespace Domain.Primitives;

public abstract class Entity(Guid id) : IEquatable<Entity> {
    protected Guid Id { get; } = id;

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);

    public override bool Equals(object? obj) => Equals(obj as Entity);

    public bool Equals(Entity? other) =>
        other is not null && GetType() == other.GetType() && Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id) * 42;
}
