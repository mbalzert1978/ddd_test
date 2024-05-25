namespace Domain;

public abstract class Entity(Guid id) : IEquatable<Entity>
{
    protected Guid Id { get; private init; } = id;

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null || right is null)
            return false;

        return Equals(left, right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (obj.GetType() != GetType())
            return false;
        if (obj is not Entity entity)
            return false;

        return Id == entity.Id;
    }

    public bool Equals(Entity? other)
    {
        if (other is null)
            return false;
        if (other.GetType() != GetType())
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 41;
    }
}
