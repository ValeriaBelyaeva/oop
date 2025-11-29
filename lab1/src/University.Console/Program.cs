using University.Application;
using University.Builders;
using University.Domain;
using University.Infrastructure.InMemory;

var courseRepo  = new InMemoryCourseRepository(useSingleton: true);
var teacherRepo = new InMemoryTeacherRepository(useSingleton: true);
var studentRepo = new InMemoryStudentRepository(useSingleton: true);

var svc = new CourseService(courseRepo, teacherRepo, studentRepo);

var teacher = new Teacher(Guid.NewGuid(), "Иван Петров");
teacherRepo.Add(teacher);

var course = CourseBuilder.Online()
    .Titled("C# для начинающих")
    .OnPlatform("Moodle")
    .WithUrl("https://moodle.university/intro-csharp")
    .Build();

svc.AddCourse(course);
svc.AssignTeacher(course.Id, teacher.Id);

Console.WriteLine($"Курс: {course.Title}, преподаватель: {teacher.FullName}");

// При необходимости очистить общее хранилище:
// DataStore.Instance.Clear();

