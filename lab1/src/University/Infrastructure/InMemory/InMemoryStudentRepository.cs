using University.Application.Abstractions;
using University.Domain;

namespace University.Infrastructure.InMemory;

public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly Dictionary<Guid, Student> _store;

    public InMemoryStudentRepository(bool useSingleton = false)
    {
        _store = useSingleton
            ? DataStore.Instance.Students
            : new Dictionary<Guid, Student>();
    }

    public void Add(Student student)
    {
        if (_store.ContainsKey(student.Id))
            throw new ValidationException($"Student with id {student.Id} already exists.");
        _store[student.Id] = student;
    }

    public Student? Get(Guid id) => _store.TryGetValue(id, out var s) ? s : null;

    public bool Exists(Guid id) => _store.ContainsKey(id);
}
