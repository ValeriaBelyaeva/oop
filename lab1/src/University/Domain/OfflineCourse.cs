namespace University.Domain;

public sealed class OfflineCourse : Course
{
    public string Campus { get; }
    public string Room { get; }
    public int Capacity { get; }

    public OfflineCourse(Guid id, string title, string? description, string campus, string room, int capacity)
        : base(id, title, description)
    {
        if (string.IsNullOrWhiteSpace(campus)) throw new ValidationException("Campus cannot be empty.");
        if (string.IsNullOrWhiteSpace(room)) throw new ValidationException("Room cannot be empty.");
        if (capacity <= 0) throw new ValidationException("Capacity must be positive.");

        Campus = campus.Trim();
        Room = room.Trim();
        Capacity = capacity;
    }

    protected override void ValidateEnrollment(Guid studentId, int newCount)
    {
        if (newCount > Capacity) throw new BusinessRuleException($"Capacity exceeded ({Capacity}).");
    }
}
