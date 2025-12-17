namespace University.Domain;

public sealed class Teacher
{
    public Guid Id { get; }
    public string FullName { get; }

    public Teacher(Guid id, string fullName)
    {
        if (id == Guid.Empty) throw new ValidationException("Teacher id cannot be empty.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ValidationException("Teacher name cannot be empty.");
        Id = id;
        FullName = fullName.Trim(); // трим от пробелов на концах
    }
}
