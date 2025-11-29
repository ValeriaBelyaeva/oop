namespace University.Domain;

public sealed class Student
{
    public Guid Id { get; }
    public string FullName { get; }
    public string Email { get; }

    public Student(Guid id, string fullName, string email)
    {
        if (id == Guid.Empty) throw new ValidationException("Student id cannot be empty.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ValidationException("Student name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email cannot be empty.");
        try { var _ = new System.Net.Mail.MailAddress(email); }
        catch { throw new ValidationException("Email is invalid."); }

        Id = id;
        FullName = fullName.Trim();
        Email = email.Trim();
    }
}
