namespace ReferenceApplicationArchitecture.BusinessLogic;

using ReferenceApplicationArchitecture.Data;
using ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// Encapsulates task-related rules and validation.
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly Func<DateTime> _clock;

    public TaskService(ITaskRepository repository, Func<DateTime>? clock = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock ?? (() => DateTime.UtcNow);
    }

    public TaskItem AddTask(string title)
    {
        var normalizedTitle = NormalizeTitle(title);
        var task = new TaskItem(Guid.NewGuid(), normalizedTitle, _clock());
        return _repository.Add(task);
    }

    public TaskItem CompleteTask(Guid id)
    {
        var existing = _repository.GetById(id) ?? throw new InvalidOperationException("Task not found.");
        if (existing.IsCompleted)
        {
            return existing;
        }

        existing.MarkCompleted(_clock());
        return _repository.Update(existing);
    }

    public IReadOnlyCollection<TaskItem> GetAll()
    {
        return _repository.GetAll();
    }

    public IReadOnlyCollection<TaskItem> GetOpen()
    {
        return _repository.GetAll().Where(t => !t.IsCompleted).ToArray();
    }

    public bool Remove(Guid id)
    {
        return _repository.Remove(id);
    }

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        }

        return title.Trim();
    }
}
