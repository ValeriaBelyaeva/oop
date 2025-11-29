namespace University.Domain;

public abstract class Course
{
    private readonly HashSet<Guid> _studentIds = new();

    public Guid Id { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Guid? TeacherId { get; private set; }
    public IReadOnlyCollection<Guid> StudentIds => _studentIds;

    protected Course(Guid id, string title, string? description)
    {
        if (id == Guid.Empty) throw new ValidationException("Course id cannot be empty.");
        if (string.IsNullOrWhiteSpace(title)) throw new ValidationException("Course title cannot be empty.");
        Id = id;
        Title = title.Trim();
        Description = description?.Trim();
    }

    public void AssignTeacher(Guid teacherId)
    {
        if (teacherId == Guid.Empty) throw new ValidationException("Teacher id cannot be empty.");
        TeacherId = teacherId;
    }

    public void RemoveTeacher() => TeacherId = null;

    public void EnrollStudent(Guid studentId)
    {
        if (studentId == Guid.Empty) throw new ValidationException("Student id cannot be empty.");
        if (_studentIds.Contains(studentId)) throw new BusinessRuleException("Student already enrolled.");
        ValidateEnrollment(studentId, _studentIds.Count + 1);
        _studentIds.Add(studentId);
    }

    public void UnenrollStudent(Guid studentId) => _studentIds.Remove(studentId);

    /// <summary>Переопределяйте для специфических правил (например, вместимость аудитории)</summary>
    protected virtual void ValidateEnrollment(Guid studentId, int newCount) { }
}
