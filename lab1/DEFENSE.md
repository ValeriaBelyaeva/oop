# 🎓 ПОЛНАЯ ПОДГОТОВКА К ЗАЩИТЕ ЛАБОРАТОРНОЙ РАБОТЫ №1

## Система управления курсами и преподавателями университета

---

# 📁 ЧАСТЬ 1: АРХИТЕКТУРА ПРОЕКТА

## Общая структура решения

```
lab1/
├── University.sln                    # Solution файл
├── src/
│   ├── University/                   # Основная библиотека классов
│   │   ├── Domain/                   # Доменный слой (сущности)
│   │   ├── Application/              # Слой приложения (бизнес-логика)
│   │   ├── Infrastructure/           # Инфраструктурный слой (хранилища)
│   │   └── Builders/                 # Паттерн Builder
│   └── University.Console/           # Консольное приложение (демонстрация)
└── tests/
    └── University.Tests/             # Unit-тесты (xUnit)
```

## Архитектурный паттерн: Clean Architecture (Чистая архитектура)

```
┌─────────────────────────────────────────────────────────────┐
│                    University.Console                        │
│                   (Точка входа, UI)                         │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     Infrastructure                           │
│         (InMemory репозитории, DataStore)                   │
│    Реализует интерфейсы из Application                      │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Application                             │
│         (CourseService, интерфейсы репозиториев)            │
│    Содержит бизнес-логику, зависит от Domain                │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                        Domain                                │
│    (Course, Student, Teacher, Exceptions)                   │
│    Ядро системы — не зависит ни от чего                     │
└─────────────────────────────────────────────────────────────┘
```

**Принцип зависимостей:** Внешние слои зависят от внутренних, но НЕ наоборот. Domain ничего не знает об Infrastructure.

---

# 📦 ЧАСТЬ 2: ДОМЕННЫЙ СЛОЙ (Domain)

## 2.1 Абстрактный класс Course

```csharp
namespace University.Domain;

public abstract class Course
{
    private readonly HashSet<Guid> _studentIds = new();

    public Guid Id { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Guid? TeacherId { get; private set; }
    public IReadOnlyCollection<Guid> StudentIds => _studentIds;

    protected Course(Guid id, string title, string? description)
    {
        if (id == Guid.Empty) throw new ValidationException("Course id cannot be empty.");
        if (string.IsNullOrWhiteSpace(title)) throw new ValidationException("Course title cannot be empty.");
        Id = id;
        Title = title.Trim();
        Description = description?.Trim();
    }

    public void AssignTeacher(Guid teacherId)
    {
        if (teacherId == Guid.Empty) throw new ValidationException("Teacher id cannot be empty.");
        TeacherId = teacherId;
    }

    public void RemoveTeacher() => TeacherId = null;

    public void EnrollStudent(Guid studentId)
    {
        if (studentId == Guid.Empty) throw new ValidationException("Student id cannot be empty.");
        if (_studentIds.Contains(studentId)) throw new BusinessRuleException("Student already enrolled.");
        ValidateEnrollment(studentId, _studentIds.Count + 1);
        _studentIds.Add(studentId);
    }

    public void UnenrollStudent(Guid studentId) => _studentIds.Remove(studentId);

    /// <summary>Переопределяйте для специфических правил (например, вместимость аудитории)</summary>
    protected virtual void ValidateEnrollment(Guid studentId, int newCount) { }
}
```

### Что здесь важно понимать:

| Элемент | Объяснение |
|---------|------------|
| `abstract class` | Нельзя создать экземпляр напрямую — только через наследников |
| `private readonly HashSet<Guid>` | Инкапсуляция — список студентов нельзя изменить извне |
| `IReadOnlyCollection<Guid>` | Предоставляем только чтение, не позволяем модифицировать коллекцию |
| `protected Course(...)` | Конструктор защищённый — вызывается только из наследников |
| `protected virtual void ValidateEnrollment()` | **Точка расширения** для полиморфизма |

### ООП-концепции в этом классе:

- **Инкапсуляция**: `_studentIds` — приватное поле, доступ через `StudentIds`
- **Абстракция**: Класс абстрактный, определяет общий контракт для всех курсов
- **Полиморфизм**: Метод `ValidateEnrollment` — виртуальный, переопределяется в наследниках

---

## 2.2 OnlineCourse — онлайн-курс

```csharp
namespace University.Domain;

public sealed class OnlineCourse : Course
{
    public string Platform { get; }
    public Uri Url { get; }

    public OnlineCourse(Guid id, string title, string? description, string platform, string url)
        : base(id, title, description)
    {
        if (string.IsNullOrWhiteSpace(platform)) throw new ValidationException("Platform cannot be empty.");
        if (string.IsNullOrWhiteSpace(url)) throw new ValidationException("URL cannot be empty.");
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)) throw new ValidationException("URL is invalid.");

        Platform = platform.Trim();
        Url = uri;
    }
}
```

### Особенности:

| Элемент | Объяснение |
|---------|------------|
| `sealed class` | Запрещает дальнейшее наследование |
| `: base(id, title, description)` | Вызов конструктора базового класса |
| `Platform`, `Url` | **Уникальные характеристики** онлайн-курса |
| `Uri.TryCreate()` | Валидация URL при создании |

**Не переопределяет `ValidateEnrollment`** — онлайн-курсы не ограничены по вместимости!

---

## 2.3 OfflineCourse — офлайн-курс

```csharp
namespace University.Domain;

public sealed class OfflineCourse : Course
{
    public string Campus { get; }
    public string Room { get; }
    public int Capacity { get; }

    public OfflineCourse(Guid id, string title, string? description, string campus, string room, int capacity)
        : base(id, title, description)
    {
        if (string.IsNullOrWhiteSpace(campus)) throw new ValidationException("Campus cannot be empty.");
        if (string.IsNullOrWhiteSpace(room)) throw new ValidationException("Room cannot be empty.");
        if (capacity <= 0) throw new ValidationException("Capacity must be positive.");

        Campus = campus.Trim();
        Room = room.Trim();
        Capacity = capacity;
    }

    protected override void ValidateEnrollment(Guid studentId, int newCount)
    {
        if (newCount > Capacity) throw new BusinessRuleException($"Capacity exceeded ({Capacity}).");
    }
}
```

### Особенности:

| Элемент | Объяснение |
|---------|------------|
| `Campus`, `Room`, `Capacity` | **Уникальные характеристики** офлайн-курса |
| `protected override void ValidateEnrollment()` | **ПОЛИМОРФИЗМ!** Переопределяем поведение |

### Как работает полиморфизм здесь:

```csharp
// В базовом классе Course:
public void EnrollStudent(Guid studentId)
{
    // ...
    ValidateEnrollment(studentId, _studentIds.Count + 1);  // ← вызывается виртуальный метод
    // ...
}

// Для OnlineCourse — вызовется пустой метод из Course (нет ограничений)
// Для OfflineCourse — вызовется переопределённый метод с проверкой Capacity
```

---

## 2.4 Student — студент

```csharp
namespace University.Domain;

public sealed class Student
{
    public Guid Id { get; }
    public string FullName { get; }
    public string Email { get; }

    public Student(Guid id, string fullName, string email)
    {
        if (id == Guid.Empty) throw new ValidationException("Student id cannot be empty.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ValidationException("Student name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email cannot be empty.");
        try { var _ = new System.Net.Mail.MailAddress(email); }
        catch { throw new ValidationException("Email is invalid."); }

        Id = id;
        FullName = fullName.Trim();
        Email = email.Trim();
    }
}
```

### Валидация email:
Используется `System.Net.Mail.MailAddress` — стандартный класс .NET для проверки формата email.

---

## 2.5 Teacher — преподаватель

```csharp
namespace University.Domain;

public sealed class Teacher
{
    public Guid Id { get; }
    public string FullName { get; }

    public Teacher(Guid id, string fullName)
    {
        if (id == Guid.Empty) throw new ValidationException("Teacher id cannot be empty.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ValidationException("Teacher name cannot be empty.");
        Id = id;
        FullName = fullName.Trim();
    }
}
```

---

## 2.6 Exceptions — иерархия исключений

```csharp
namespace University.Domain;

public abstract class UniversityException : Exception
{
    protected UniversityException(string message) : base(message) { }
}

public sealed class EntityNotFoundException : UniversityException
{
    public EntityNotFoundException(string entity, Guid id)
        : base($"{entity} with id {id} was not found.") { }
}

public sealed class ValidationException : UniversityException
{
    public ValidationException(string message) : base(message) { }
}

public sealed class BusinessRuleException : UniversityException
{
    public BusinessRuleException(string message) : base(message) { }
}
```

### Три типа ошибок:

| Исключение | Когда выбрасывается | Пример |
|------------|---------------------|--------|
| `ValidationException` | Некорректные входные данные | Пустое имя, невалидный email |
| `EntityNotFoundException` | Сущность не найдена в хранилище | Курс с ID не существует |
| `BusinessRuleException` | Нарушение бизнес-правила | Студент уже записан, превышена вместимость |

---

# ⚙️ ЧАСТЬ 3: СЛОЙ ПРИЛОЖЕНИЯ (Application)

## 3.1 Интерфейсы репозиториев

### ICourseRepository

```csharp
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
```

### IStudentRepository

```csharp
using University.Domain;

namespace University.Application.Abstractions;

public interface IStudentRepository
{
    void Add(Student student);
    Student? Get(Guid id);
    bool Exists(Guid id);
}
```

### ITeacherRepository

```csharp
using University.Domain;

namespace University.Application.Abstractions;

public interface ITeacherRepository
{
    void Add(Teacher teacher);
    Teacher? Get(Guid id);
    bool Exists(Guid id);
}
```

### Зачем нужны интерфейсы?

1. **Принцип инверсии зависимостей (DIP)** — сервис зависит от абстракции, а не от конкретной реализации
2. **Тестируемость** — можно подставить mock-репозиторий в тестах
3. **Гибкость** — легко заменить InMemory на базу данных, не меняя бизнес-логику

---

## 3.2 CourseService — главный сервис

```csharp
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
```

### Методы сервиса:

| Метод | Что делает | Требование ЛР |
|-------|-----------|---------------|
| `AddCourse()` | Добавляет курс | Требование 1 |
| `RemoveCourse()` | Удаляет курс | Требование 1 |
| `AssignTeacher()` | Назначает преподавателя | Требование 1 |
| `EnrollStudent()` | Записывает студента на курс | Требование 1 |
| `GetStudentsOfCourse()` | Возвращает студентов курса | Требование 1 |
| `GetCoursesByTeacher()` | Возвращает курсы преподавателя | Требование 3 |

### Конструктор с инъекцией зависимостей:

```csharp
public CourseService(ICourseRepository courseRepo, ITeacherRepository teacherRepo, IStudentRepository studentRepo)
```

Принимает **интерфейсы**, а не конкретные классы — **Dependency Injection**.

---

# 🗄️ ЧАСТЬ 4: ИНФРАСТРУКТУРНЫЙ СЛОЙ (Infrastructure)

## 4.1 InMemoryCourseRepository

```csharp
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
```

**Реализует интерфейс `ICourseRepository`** — можно заменить на реальную БД без изменения бизнес-логики.

---

## 4.2 InMemoryStudentRepository

```csharp
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
```

---

## 4.3 InMemoryTeacherRepository

```csharp
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
```

---

## 4.4 DataStore — паттерн Singleton

```csharp
using System;
using System.Collections.Generic;
using University.Domain;

namespace University.Infrastructure.InMemory;

public sealed class DataStore
{
    private static readonly Lazy<DataStore> _instance = new(() => new DataStore());
    public static DataStore Instance => _instance.Value;

    public Dictionary<Guid, Course>  Courses  { get; } = new();
    public Dictionary<Guid, Teacher> Teachers { get; } = new();
    public Dictionary<Guid, Student> Students { get; } = new();

    private DataStore() { }

    public void Clear()
    {
        Courses.Clear();
        Teachers.Clear();
        Students.Clear();
    }
}
```

### Паттерн Singleton:

| Элемент | Роль |
|---------|------|
| `private DataStore()` | Приватный конструктор — нельзя создать извне |
| `static readonly Lazy<DataStore>` | Ленивая инициализация, потокобезопасность |
| `public static DataStore Instance` | Единственная точка доступа |

**Зачем Singleton здесь?** Общее хранилище данных для всего приложения. В консольном приложении репозитории используют `useSingleton: true`.

---

# 🏗️ ЧАСТЬ 5: ПАТТЕРН BUILDER

```csharp
using University.Domain;

namespace University.Builders;

public static class CourseBuilder
{
    public static OnlineCourseBuilder Online() => new OnlineCourseBuilder();
    public static OfflineCourseBuilder Offline() => new OfflineCourseBuilder();
}

public sealed class OnlineCourseBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Untitled";
    private string? _description;
    private string _platform = "Platform";
    private string _url = "https://example.com";

    public OnlineCourseBuilder WithId(Guid id) { _id = id; return this; }
    public OnlineCourseBuilder Titled(string title) { _title = title; return this; }
    public OnlineCourseBuilder DescribedAs(string? description) { _description = description; return this; }
    public OnlineCourseBuilder OnPlatform(string platform) { _platform = platform; return this; }
    public OnlineCourseBuilder WithUrl(string url) { _url = url; return this; }

    public OnlineCourse Build() => new OnlineCourse(_id, _title, _description, _platform, _url);
}

public sealed class OfflineCourseBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Untitled";
    private string? _description;
    private string _campus = "Main";
    private string _room = "A-101";
    private int _capacity = 30;

    public OfflineCourseBuilder WithId(Guid id) { _id = id; return this; }
    public OfflineCourseBuilder Titled(string title) { _title = title; return this; }
    public OfflineCourseBuilder DescribedAs(string? description) { _description = description; return this; }
    public OfflineCourseBuilder AtCampus(string campus) { _campus = campus; return this; }
    public OfflineCourseBuilder InRoom(string room) { _room = room; return this; }
    public OfflineCourseBuilder WithCapacity(int capacity) { _capacity = capacity; return this; }

    public OfflineCourse Build() => new OfflineCourse(_id, _title, _description, _campus, _room, _capacity);
}
```

### Паттерн Builder — что это?

**Порождающий паттерн**, который позволяет создавать сложные объекты пошагово.

### Fluent Interface (цепочка вызовов):

```csharp
var course = CourseBuilder.Online()
    .Titled("C# для начинающих")
    .OnPlatform("Moodle")
    .WithUrl("https://moodle.university/intro-csharp")
    .Build();
```

### Преимущества Builder:

1. **Читаемость** — понятно, что создаётся
2. **Значения по умолчанию** — не нужно указывать все параметры
3. **Неизменяемость результата** — объект создаётся целиком в `Build()`
4. **Упрощение тестов** — легко создавать объекты с разными параметрами

---

# 🖥️ ЧАСТЬ 6: КОНСОЛЬНОЕ ПРИЛОЖЕНИЕ

```csharp
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
```

### Что демонстрирует:

1. Создание репозиториев с общим хранилищем (Singleton)
2. Создание сервиса через Dependency Injection
3. Использование Builder для создания курса
4. Работа с сервисом (добавление курса, назначение преподавателя)

---

# 🧪 ЧАСТЬ 7: UNIT-ТЕСТЫ

```csharp
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
```

### Тесты и что они проверяют:

| Тест | Проверяет | Требование |
|------|-----------|------------|
| `Can_Add_And_Remove_Course` | Добавление и удаление курсов | Требование 1 |
| `Assign_Teacher_And_Get_Courses_By_Teacher` | Назначение преподавателя + получение его курсов | Требование 1, 3 |
| `Enroll_Student_In_Online_Course` | Запись студента на онлайн-курс | Требование 1 |
| `Offline_Capacity_Is_Enforced` | Проверка вместимости офлайн-курса | **Полиморфизм!** |
| `Duplicate_Enrollment_Throws` | Запрет повторной записи | Бизнес-правило |
| `Assigning_NonExisting_Teacher_Throws` | Ошибка при несуществующем преподавателе | Валидация |

### Структура теста (AAA-паттерн):

```csharp
[Fact]
public void Offline_Capacity_Is_Enforced()
{
    // ARRANGE — подготовка
    var offline = CourseBuilder.Offline()
        .Titled("Physics")
        .WithCapacity(2)
        .Build();
    _service.AddCourse(offline);
    
    var s1 = new Student(Guid.NewGuid(), "S1", "s1@example.com");
    var s2 = new Student(Guid.NewGuid(), "S2", "s2@example.com");
    var s3 = new Student(Guid.NewGuid(), "S3", "s3@example.com");
    _studentRepo.Add(s1); _studentRepo.Add(s2); _studentRepo.Add(s3);

    // ACT — действие
    _service.EnrollStudent(offline.Id, s1.Id);
    _service.EnrollStudent(offline.Id, s2.Id);

    // ASSERT — проверка
    Assert.Throws<BusinessRuleException>(() => _service.EnrollStudent(offline.Id, s3.Id));
}
```

---

# 🎯 ЧАСТЬ 8: СООТВЕТСТВИЕ ТРЕБОВАНИЯМ ЛР

## Обязательные требования

| № | Требование | ✅ Реализация |
|---|-----------|--------------|
| 1 | Добавлять курсы | `CourseService.AddCourse()` |
| 1 | Удалять курсы | `CourseService.RemoveCourse()` |
| 1 | Назначать преподавателей | `CourseService.AssignTeacher()` |
| 1 | Хранить студентов на курсе | `Course._studentIds` + `EnrollStudent()` |
| 1 | Получать студентов курса | `CourseService.GetStudentsOfCourse()` |
| 2 | Онлайн-курсы | `OnlineCourse` (Platform, Url) |
| 2 | Офлайн-курсы | `OfflineCourse` (Campus, Room, Capacity) |
| 3 | Курсы преподавателя | `CourseService.GetCoursesByTeacher()` |
| 4 | Unit-тесты xUnit | `CourseServiceTests` — 6 тестов |

## Паттерны (задание со звёздочкой)

| Паттерн | ✅ Реализация |
|---------|--------------|
| Builder | `CourseBuilder`, `OnlineCourseBuilder`, `OfflineCourseBuilder` |
| Singleton | `DataStore.Instance` |

## Цели ЛР (ООП-концепции)

| Концепция | ✅ Где применено |
|-----------|-----------------|
| **Наследование** | `OnlineCourse : Course`, `OfflineCourse : Course` |
| **Полиморфизм** | `ValidateEnrollment()` переопределён в `OfflineCourse` |
| **Абстракция** | `abstract class Course`, интерфейсы репозиториев |
| **Интерфейсы** | `ICourseRepository`, `IStudentRepository`, `ITeacherRepository` |
| **Инкапсуляция** | Приватные поля, публичные свойства только для чтения |

---

# ❓ ЧАСТЬ 9: ВОЗМОЖНЫЕ ВОПРОСЫ НА ЗАЩИТЕ

## Вопросы по ООП

**Q: Что такое абстрактный класс и зачем он нужен?**

> A: Абстрактный класс — это класс, который нельзя инстанцировать напрямую. В проекте `Course` — абстрактный, потому что не существует "просто курса", есть только онлайн или офлайн. Он определяет общий контракт и поведение для всех типов курсов.

---

**Q: Чем абстрактный класс отличается от интерфейса?**

> A: Абстрактный класс может содержать реализацию методов и поля, интерфейс — только сигнатуры (до C# 8). Класс может наследовать только один абстрактный класс, но реализовывать много интерфейсов. В проекте `Course` содержит логику (`EnrollStudent`), поэтому это класс, а не интерфейс.

---

**Q: Покажите полиморфизм в проекте.**

> A: Метод `ValidateEnrollment()` в базовом классе `Course` — виртуальный и пустой. В `OfflineCourse` он переопределён для проверки вместимости. При вызове `EnrollStudent()` срабатывает нужная версия метода в зависимости от типа объекта.

```csharp
// Базовый класс
protected virtual void ValidateEnrollment(Guid studentId, int newCount) { }

// OfflineCourse
protected override void ValidateEnrollment(Guid studentId, int newCount)
{
    if (newCount > Capacity) throw new BusinessRuleException($"Capacity exceeded ({Capacity}).");
}
```

---

**Q: Что такое инкапсуляция и где она применена?**

> A: Инкапсуляция — скрытие внутренней реализации. Поле `_studentIds` приватное, снаружи доступно только через `StudentIds` (read-only). Нельзя напрямую изменить список студентов — только через методы `EnrollStudent`/`UnenrollStudent`.

---

**Q: Что такое `sealed` класс?**

> A: `sealed` запрещает наследование от класса. `OnlineCourse` и `OfflineCourse` помечены как `sealed`, потому что они — конечные реализации, дальнейшее расширение не предполагается.

---

## Вопросы по паттернам

**Q: Зачем нужен паттерн Builder?**

> A: Builder упрощает создание объектов с множеством параметров. Вместо конструктора с 6 параметрами — читаемая цепочка вызовов. Также позволяет задать значения по умолчанию и создавать объекты пошагово.

---

**Q: Как работает Singleton в проекте?**

> A: `DataStore` имеет приватный конструктор и статическое свойство `Instance`. `Lazy<T>` гарантирует создание единственного экземпляра при первом обращении. Это потокобезопасно благодаря `Lazy`.

```csharp
private static readonly Lazy<DataStore> _instance = new(() => new DataStore());
public static DataStore Instance => _instance.Value;
private DataStore() { }  // приватный конструктор
```

---

**Q: Какие проблемы у Singleton?**

> A: Singleton затрудняет тестирование (глобальное состояние), нарушает принцип единственной ответственности. В проекте это решено через параметр `useSingleton` — в тестах используется изолированное хранилище.

---

**Q: Зачем использовать интерфейсы репозиториев?**

> A: Принцип инверсии зависимостей (DIP из SOLID). `CourseService` зависит от `ICourseRepository`, а не от `InMemoryCourseRepository`. Можно легко заменить хранилище на базу данных, не меняя бизнес-логику.

---

## Вопросы по архитектуре

**Q: Почему разделили на слои (Domain, Application, Infrastructure)?**

> A: Clean Architecture. Domain не зависит ни от чего — это ядро с бизнес-сущностями. Application содержит бизнес-логику (сервисы). Infrastructure — детали реализации (хранилище). Такое разделение упрощает тестирование и поддержку.

---

**Q: Почему исключения в Domain, а не в Application?**

> A: Исключения — часть доменной модели. `ValidationException` выбрасывается при создании сущностей (в конструкторах). Domain должен сам защищать свои инварианты.

---

**Q: Что такое Dependency Injection и где он применён?**

> A: DI — передача зависимостей через конструктор вместо создания внутри класса. `CourseService` принимает интерфейсы репозиториев в конструкторе, а не создаёт их сам. Это позволяет подставлять разные реализации.

```csharp
public CourseService(ICourseRepository courseRepo, ITeacherRepository teacherRepo, IStudentRepository studentRepo)
```

---

## Вопросы по тестам

**Q: Почему в тестах не используется `useSingleton: true`?**

> A: Тесты должны быть изолированы. Каждый тест создаёт свои репозитории, чтобы не влиять на другие тесты. Singleton в тестах — антипаттерн, приводит к "грязным" тестам.

---

**Q: Что проверяет тест `Offline_Capacity_Is_Enforced`?**

> A: Проверяет полиморфизм. При записи третьего студента на курс с вместимостью 2 должен выброситься `BusinessRuleException`. Это работает благодаря переопределению `ValidateEnrollment` в `OfflineCourse`.

---

**Q: Что такое AAA-паттерн в тестах?**

> A: Arrange-Act-Assert. Arrange — подготовка данных, Act — выполнение действия, Assert — проверка результата. Структурирует тесты и делает их читаемыми.

---

**Q: Зачем нужен атрибут `[Fact]`?**

> A: Это атрибут xUnit, который помечает метод как тестовый. Test runner найдёт и выполнит все методы с этим атрибутом.

---

## Вопросы по C#

**Q: Что такое `Guid` и зачем он используется?**

> A: `Guid` (Globally Unique Identifier) — 128-битный уникальный идентификатор. Используется вместо `int` для ID, потому что не требует централизованной генерации (БД) и уникален глобально.

---

**Q: Что делает `?.` (null-conditional operator)?**

> A: Безопасный доступ к членам. `description?.Trim()` вернёт `null`, если `description` равен `null`, вместо выброса `NullReferenceException`.

---

**Q: Что значит `??` (null-coalescing operator)?**

> A: Возвращает левый операнд, если он не `null`, иначе — правый. `_courseRepo.Get(courseId) ?? throw new EntityNotFoundException(...)` выбросит исключение, если курс не найден.

---

**Q: Зачем `IReadOnlyCollection<Guid>` вместо `HashSet<Guid>`?**

> A: Инкапсуляция. Возвращаем интерфейс только для чтения, чтобы внешний код не мог модифицировать коллекцию напрямую (например, вызвать `Add` или `Clear`).

---

# 🚀 ЧАСТЬ 10: КОМАНДЫ ДЛЯ ЗАПУСКА

```bash
# Сборка проекта
dotnet build

# Запуск консольного приложения
dotnet run --project src/University.Console

# Запуск тестов
dotnet test

# Запуск тестов с подробным выводом
dotnet test --logger "console;verbosity=detailed"
```

---

# 📊 ДИАГРАММА КЛАССОВ

```
                    ┌──────────────────────┐
                    │   <<abstract>>       │
                    │       Course         │
                    ├──────────────────────┤
                    │ - _studentIds        │
                    │ + Id                 │
                    │ + Title              │
                    │ + Description        │
                    │ + TeacherId          │
                    │ + StudentIds         │
                    ├──────────────────────┤
                    │ + AssignTeacher()    │
                    │ + RemoveTeacher()    │
                    │ + EnrollStudent()    │
                    │ + UnenrollStudent()  │
                    │ # ValidateEnrollment()│
                    └──────────┬───────────┘
                               │
              ┌────────────────┴────────────────┐
              │                                 │
              ▼                                 ▼
┌─────────────────────┐           ┌─────────────────────┐
│   <<sealed>>        │           │   <<sealed>>        │
│   OnlineCourse      │           │   OfflineCourse     │
├─────────────────────┤           ├─────────────────────┤
│ + Platform          │           │ + Campus            │
│ + Url               │           │ + Room              │
│                     │           │ + Capacity          │
├─────────────────────┤           ├─────────────────────┤
│                     │           │ # ValidateEnrollment()│
└─────────────────────┘           └─────────────────────┘


┌─────────────────────┐           ┌─────────────────────┐
│      Student        │           │      Teacher        │
├─────────────────────┤           ├─────────────────────┤
│ + Id                │           │ + Id                │
│ + FullName          │           │ + FullName          │
│ + Email             │           └─────────────────────┘
└─────────────────────┘


                    ┌──────────────────────┐
                    │   <<interface>>      │
                    │  ICourseRepository   │
                    ├──────────────────────┤
                    │ + Add()              │
                    │ + Get()              │
                    │ + Remove()           │
                    │ + GetAll()           │
                    │ + GetByTeacher()     │
                    │ + Update()           │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │ InMemoryCourse       │
                    │    Repository        │
                    └──────────────────────┘
```

---

**Удачи на защите!** 🎓

