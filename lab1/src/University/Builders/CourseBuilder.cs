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
