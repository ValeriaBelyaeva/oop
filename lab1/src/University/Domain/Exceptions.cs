namespace University.Domain;

public abstract class UniversityException : Exception
{
    protected UniversityException(string message) : base(message) { }
}

public sealed class EntityNotFoundException : UniversityException
{
    public EntityNotFoundException(string entity, Guid id)
        : base($"{entity} with id {id} was not found.") { }
}

public sealed class ValidationException : UniversityException
{
    public ValidationException(string message) : base(message) { }
}

public sealed class BusinessRuleException : UniversityException
{
    public BusinessRuleException(string message) : base(message) { }
}
