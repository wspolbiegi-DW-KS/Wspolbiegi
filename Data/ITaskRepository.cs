namespace ReferenceApplicationArchitecture.Data;

using ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// Abstraction over task storage to enable swapping implementations and ease testing.
/// </summary>
public interface ITaskRepository
{
    TaskItem Add(TaskItem task);
    TaskItem? GetById(Guid id);
    IReadOnlyCollection<TaskItem> GetAll();
    TaskItem Update(TaskItem task);
    bool Remove(Guid id);
}
