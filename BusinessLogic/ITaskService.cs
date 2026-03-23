namespace ReferenceApplicationArchitecture.BusinessLogic;

using ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// Business operations for managing tasks, independent from storage and UI.
/// </summary>
public interface ITaskService
{
    TaskItem AddTask(string title);
    TaskItem CompleteTask(Guid id);
    IReadOnlyCollection<TaskItem> GetAll();
    IReadOnlyCollection<TaskItem> GetOpen();
    bool Remove(Guid id);
}
