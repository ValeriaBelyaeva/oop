using University.Domain;

namespace University.Application.Abstractions;

public interface ITeacherRepository
{
    void Add(Teacher teacher);
    Teacher? Get(Guid id);
    bool Exists(Guid id);
}
