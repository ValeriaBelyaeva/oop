using University.Domain;

namespace University.Application.Abstractions;

public interface IStudentRepository
{
    void Add(Student student);
    Student? Get(Guid id);
    bool Exists(Guid id);
}
