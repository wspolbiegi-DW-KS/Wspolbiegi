namespace ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// Domain model representing a single unit of work displayed in the UI.
/// </summary>
public class TaskItem
{
    public TaskItem(Guid id, string title, DateTime createdAt, bool isCompleted = false, DateTime? completedAt = null)
    {
        Id = id;
        Title = title;
        CreatedAt = createdAt;
        IsCompleted = isCompleted;
        CompletedAt = completedAt;
    }

    public Guid Id { get; }

    public string Title { get; }

    public DateTime CreatedAt { get; }

    public bool IsCompleted { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public TaskItem MarkCompleted(DateTime completedAt)
    {
        IsCompleted = true;
        CompletedAt = completedAt;
        return this;
    }
}
