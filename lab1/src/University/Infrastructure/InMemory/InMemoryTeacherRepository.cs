using University.Application.Abstractions;
using University.Domain;

namespace University.Infrastructure.InMemory;

public sealed class InMemoryTeacherRepository : ITeacherRepository
{
    private readonly Dictionary<Guid, Teacher> _store;

    public InMemoryTeacherRepository(bool useSingleton = false)
    {
        _store = useSingleton
            ? DataStore.Instance.Teachers
            : new Dictionary<Guid, Teacher>();
    }

    public void Add(Teacher teacher)
    {
        if (_store.ContainsKey(teacher.Id))
            throw new ValidationException($"Teacher with id {teacher.Id} already exists.");
        _store[teacher.Id] = teacher;
    }

    public Teacher? Get(Guid id) => _store.TryGetValue(id, out var t) ? t : null;

    public bool Exists(Guid id) => _store.ContainsKey(id);
}
