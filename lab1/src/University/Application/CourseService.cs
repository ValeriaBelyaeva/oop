using System.Linq;
using University.Application.Abstractions;
using University.Domain;

namespace University.Application;

public sealed class CourseService
{
    private readonly ICourseRepository _courseRepo;
    private readonly ITeacherRepository _teacherRepo;
    private readonly IStudentRepository _studentRepo;

    public CourseService(ICourseRepository courseRepo, ITeacherRepository teacherRepo, IStudentRepository studentRepo)
    {
        _courseRepo = courseRepo;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
    }

    public void AddCourse(Course course) => _courseRepo.Add(course);

    public void RemoveCourse(Guid courseId)
    {
        var ok = _courseRepo.Remove(courseId);
        if (!ok) throw new EntityNotFoundException("Course", courseId);
    }

    public void AssignTeacher(Guid courseId, Guid teacherId)
    {
        var course = _courseRepo.Get(courseId) ?? throw new EntityNotFoundException("Course", courseId);
        if (!_teacherRepo.Exists(teacherId)) throw new EntityNotFoundException("Teacher", teacherId);
        course.AssignTeacher(teacherId);
        _courseRepo.Update(course);
    }

    public void EnrollStudent(Guid courseId, Guid studentId)
    {
        var course = _courseRepo.Get(courseId) ?? throw new EntityNotFoundException("Course", courseId);
        if (!_studentRepo.Exists(studentId)) throw new EntityNotFoundException("Student", studentId);
        course.EnrollStudent(studentId);
        _courseRepo.Update(course);
    }

    public IReadOnlyList<Student> GetStudentsOfCourse(Guid courseId)
    {
        var course = _courseRepo.Get(courseId) ?? throw new EntityNotFoundException("Course", courseId);
        var result = new List<Student>();
        foreach (var sid in course.StudentIds)
        {
            var st = _studentRepo.Get(sid);
            if (st != null) result.Add(st);
        }
        return result;
    }

    public IReadOnlyList<Course> GetCoursesByTeacher(Guid teacherId)
    {
        if (!_teacherRepo.Exists(teacherId)) throw new EntityNotFoundException("Teacher", teacherId);
        return _courseRepo.GetByTeacher(teacherId).ToList();
    }
}
