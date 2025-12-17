using University.Application.Abstractions;
using University.Domain;

namespace University.Infrastructure.InMemory;

public sealed class InMemoryCourseRepository : ICourseRepository
{
    private readonly Dictionary<Guid, Course> _store;

    public InMemoryCourseRepository(bool useSingleton = false)
    {
        _store = useSingleton
            ? DataStore.Instance.Courses
            : new Dictionary<Guid, Course>();
    }

    public void Add(Course course)
    {
        if (_store.ContainsKey(course.Id))
            throw new ValidationException($"Course with id {course.Id} already exists.");
        _store[course.Id] = course;
    }

    public Course? Get(Guid id) => _store.TryGetValue(id, out var c) ? c : null;

    public IEnumerable<Course> GetAll() => _store.Values;

    public IEnumerable<Course> GetByTeacher(Guid teacherId) => _store.Values.Where(c => c.TeacherId == teacherId);

    public bool Remove(Guid id) => _store.Remove(id);

    public void Update(Course course)
    {
        if (!_store.ContainsKey(course.Id))
            throw new EntityNotFoundException("Course", course.Id);
        _store[course.Id] = course;
    }
}
