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

    /// <summary>Удобно для интеграционных сценариев и ручного сброса в демо.</summary>
    public void Clear()
    {
        Courses.Clear();
        Teachers.Clear();
        Students.Clear();
    }
}

