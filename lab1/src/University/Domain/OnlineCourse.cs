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
