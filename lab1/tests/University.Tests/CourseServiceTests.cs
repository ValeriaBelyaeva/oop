using System;
using Xunit;
using University.Domain;
using University.Application;
using University.Application.Abstractions;
using University.Infrastructure.InMemory;
using University.Builders;

namespace University.Tests;

public class CourseServiceTests
{
    private readonly ICourseRepository _courseRepo;
    private readonly ITeacherRepository _teacherRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _courseRepo = new InMemoryCourseRepository();
        _teacherRepo = new InMemoryTeacherRepository();
        _studentRepo = new InMemoryStudentRepository();
        _service = new CourseService(_courseRepo, _teacherRepo, _studentRepo);
    }

    [Fact]
    public void Can_Add_And_Remove_Course()
    {
        var course = CourseBuilder.Online()
            .Titled("C# 101")
            .OnPlatform("Moodle")
            .WithUrl("https://moodle.test/course/csharp")
            .Build();

        _service.AddCourse(course);
        var fromRepo = _courseRepo.Get(course.Id);

        Assert.NotNull(fromRepo);
        Assert.Equal("C# 101", fromRepo!.Title);

        _service.RemoveCourse(course.Id);
        Assert.Null(_courseRepo.Get(course.Id));
    }

    [Fact]
    public void Assign_Teacher_And_Get_Courses_By_Teacher()
    {
        var teacher = new Teacher(Guid.NewGuid(), "Dr. Smith");
        _teacherRepo.Add(teacher);

        var offline = CourseBuilder.Offline()
            .Titled("Algorithms")
            .AtCampus("North")
            .InRoom("B-201")
            .WithCapacity(20)
            .Build();

        _service.AddCourse(offline);
        _service.AssignTeacher(offline.Id, teacher.Id);

        var courses = _service.GetCoursesByTeacher(teacher.Id);
        Assert.Single(courses);
        Assert.Equal(offline.Id, courses[0].Id);
    }

    [Fact]
    public void Enroll_Student_In_Online_Course()
    {
        var student = new Student(Guid.NewGuid(), "Alice", "alice@example.com");
        _studentRepo.Add(student);

        var online = CourseBuilder.Online()
            .Titled("Databases")
            .OnPlatform("Coursera")
            .WithUrl("https://example.org/db")
            .Build();

        _service.AddCourse(online);
        _service.EnrollStudent(online.Id, student.Id);

        var students = _service.GetStudentsOfCourse(online.Id);
        Assert.Single(students);
        Assert.Equal(student.Email, students[0].Email);
    }

    [Fact]
    public void Offline_Capacity_Is_Enforced()
    {
        var offline = CourseBuilder.Offline()
            .Titled("Physics")
            .AtCampus("Main")
            .InRoom("C-10")
            .WithCapacity(2)
            .Build();

        _service.AddCourse(offline);

        var s1 = new Student(Guid.NewGuid(), "S1", "s1@example.com");
        var s2 = new Student(Guid.NewGuid(), "S2", "s2@example.com");
        var s3 = new Student(Guid.NewGuid(), "S3", "s3@example.com");
        _studentRepo.Add(s1); _studentRepo.Add(s2); _studentRepo.Add(s3);

        _service.EnrollStudent(offline.Id, s1.Id);
        _service.EnrollStudent(offline.Id, s2.Id);

        Assert.Throws<BusinessRuleException>(() => _service.EnrollStudent(offline.Id, s3.Id));
    }

    [Fact]
    public void Duplicate_Enrollment_Throws()
    {
        var student = new Student(Guid.NewGuid(), "Bob", "bob@example.com");
        _studentRepo.Add(student);

        var course = CourseBuilder.Online()
            .Titled("AI")
            .OnPlatform("edX")
            .WithUrl("https://edx.org/ai")
            .Build();

        _service.AddCourse(course);
        _service.EnrollStudent(course.Id, student.Id);

        Assert.Throws<BusinessRuleException>(() => _service.EnrollStudent(course.Id, student.Id));
    }

    [Fact]
    public void Assigning_NonExisting_Teacher_Throws()
    {
        var course = CourseBuilder.Offline()
            .Titled("Math")
            .AtCampus("Main")
            .InRoom("A-1")
            .WithCapacity(20)
            .Build();

        _service.AddCourse(course);
        var nonExistingTeacherId = Guid.NewGuid();

        Assert.Throws<EntityNotFoundException>(() => _service.AssignTeacher(course.Id, nonExistingTeacherId));
    }
}
