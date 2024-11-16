namespace Domain.Primitives;

[Serializable]
public class UnwrapFailedException(string? message) : Exception(message);
