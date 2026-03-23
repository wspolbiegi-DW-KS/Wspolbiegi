namespace ReferenceApplicationArchitecture.Data;

using System.Collections.Concurrent;
using ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// Thread-safe in-memory repository. No external storage dependencies.
/// </summary>
public class InMemoryTaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, TaskItem> _storage = new();

    public TaskItem Add(TaskItem task)
    {
        if (!_storage.TryAdd(task.Id, task))
        {
            throw new InvalidOperationException($"Task with id {task.Id} already exists.");
        }

        return task;
    }

    public TaskItem? GetById(Guid id)
    {
        _storage.TryGetValue(id, out var task);
        return task;
    }

    public IReadOnlyCollection<TaskItem> GetAll()
    {
        return _storage.Values
            .OrderBy(t => t.CreatedAt)
            .ToArray();
    }

    public TaskItem Update(TaskItem task)
    {
        _storage.AddOrUpdate(task.Id, _ => task, (_, __) => task);
        return task;
    }

    public bool Remove(Guid id)
    {
        return _storage.TryRemove(id, out _);
    }
}
