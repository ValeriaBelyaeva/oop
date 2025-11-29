using University.Domain;

namespace University.Application.Abstractions;

public interface ICourseRepository
{
    void Add(Course course);
    Course? Get(Guid id);
    bool Remove(Guid id);
    IEnumerable<Course> GetAll();
    IEnumerable<Course> GetByTeacher(Guid teacherId);
    void Update(Course course);
}
