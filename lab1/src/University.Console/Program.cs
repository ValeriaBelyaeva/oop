using University.Application;
using University.Builders;
using University.Domain;
using University.Infrastructure.InMemory;

var courseRepo  = new InMemoryCourseRepository(useSingleton: true);
var teacherRepo = new InMemoryTeacherRepository(useSingleton: true);
var studentRepo = new InMemoryStudentRepository(useSingleton: true);

var svc = new CourseService(courseRepo, teacherRepo, studentRepo);

Console.WriteLine("=== Демонстрация системы управления курсами ===\n");

// Создание преподавателей
var teacher1 = new Teacher(Guid.NewGuid(), "Иван Петров");
var teacher2 = new Teacher(Guid.NewGuid(), "Мария Сидорова");
teacherRepo.Add(teacher1);
teacherRepo.Add(teacher2);
Console.WriteLine($"Созданы преподаватели: {teacher1.FullName}, {teacher2.FullName}\n");

// Создание онлайн-курса
var onlineCourse = CourseBuilder.Online()
    .Titled("ООП")
    .DescribedAs("Основы программирования на C#")
    .OnPlatform("Moodle")
    .WithUrl("https://moodle.university/intro-csharp")
    .Build();

svc.AddCourse(onlineCourse);
svc.AssignTeacher(onlineCourse.Id, teacher1.Id);
Console.WriteLine($"Создан онлайн-курс: {onlineCourse.Title}");
Console.WriteLine($"  Платформа: {onlineCourse.Platform}, URL: {onlineCourse.Url}");
Console.WriteLine($"  Преподаватель: {teacher1.FullName}\n");

// Создание офлайн-курса
var offlineCourse = CourseBuilder.Offline()
    .Titled("Алгоритмы и структуры данных")
    .DescribedAs("Практические занятия по алгоритмам")
    .AtCampus("Главный корпус")
    .InRoom("A-201")
    .WithCapacity(2)
    .Build();

svc.AddCourse(offlineCourse);
svc.AssignTeacher(offlineCourse.Id, teacher2.Id);
Console.WriteLine($"Создан офлайн-курс: {offlineCourse.Title}");
Console.WriteLine($"  Кампус: {offlineCourse.Campus}, Аудитория: {offlineCourse.Room}");
Console.WriteLine($"  Вместимость: {offlineCourse.Capacity}");
Console.WriteLine($"  Преподаватель: {teacher2.FullName}\n");

// Создание студентов
var student1 = new Student(Guid.NewGuid(), "Алексей Иванов", "alexey@example.com");
var student2 = new Student(Guid.NewGuid(), "Елена Смирнова", "elena@example.com");
var student3 = new Student(Guid.NewGuid(), "Дмитрий Козлов", "dmitry@example.com");
studentRepo.Add(student1);
studentRepo.Add(student2);
studentRepo.Add(student3);
Console.WriteLine($"Созданы студенты: {student1.FullName}, {student2.FullName}, {student3.FullName}\n");

// Запись студентов на онлайн-курс (без ограничений)
svc.EnrollStudent(onlineCourse.Id, student1.Id);
svc.EnrollStudent(onlineCourse.Id, student2.Id);
svc.EnrollStudent(onlineCourse.Id, student3.Id);
Console.WriteLine($"Записаны студенты на онлайн-курс '{onlineCourse.Title}'\n");

// Запись студентов на офлайн-курс (с ограничением вместимости)
svc.EnrollStudent(offlineCourse.Id, student1.Id);
svc.EnrollStudent(offlineCourse.Id, student2.Id);
Console.WriteLine($"Записаны студенты на офлайн-курс '{offlineCourse.Title}'\n");

// Получение списка студентов курса
var onlineStudents = svc.GetStudentsOfCourse(onlineCourse.Id);
Console.WriteLine($"Студенты онлайн-курса '{onlineCourse.Title}':");
foreach (var student in onlineStudents)
{
    Console.WriteLine($"  - {student.FullName} ({student.Email})");
}
Console.WriteLine();

// Получение курсов преподавателя
var teacher1Courses = svc.GetCoursesByTeacher(teacher1.Id);
Console.WriteLine($"Курсы преподавателя {teacher1.FullName}:");
foreach (var course in teacher1Courses)
{
    Console.WriteLine($"  - {course.Title} ({course.GetType().Name})");
}
Console.WriteLine();

// Демонстрация ограничения вместимости
Console.WriteLine("Попытка записать третьего студента на офлайн-курс (вместимость = 2):");
try
{
    svc.EnrollStudent(offlineCourse.Id, student3.Id);
}
catch (BusinessRuleException ex)
{
    Console.WriteLine($"  Ошибка: {ex.Message}");
}
Console.WriteLine();

// Итоговая статистика
var allCourses = courseRepo.GetAll().ToList();
Console.WriteLine("=== Итоговая статистика ===");
Console.WriteLine($"Всего курсов: {allCourses.Count}");
Console.WriteLine($"  Онлайн: {allCourses.Count(c => c is OnlineCourse)}");
Console.WriteLine($"  Офлайн: {allCourses.Count(c => c is OfflineCourse)}");

// При необходимости очистить общее хранилище:
// DataStore.Instance.Clear();

