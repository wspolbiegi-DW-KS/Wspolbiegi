namespace ReferenceApplicationArchitecture.PresentationViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReferenceApplicationArchitecture.BusinessLogic;
using ReferenceApplicationArchitecture.PresentationModel;

/// <summary>
/// ViewModel exposing task operations to the WPF view.
/// </summary>
public class TaskListViewModel : INotifyPropertyChanged
{
    private readonly ITaskService _taskService;
    private string? _lastError;

    public TaskListViewModel(ITaskService taskService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        Tasks = new ObservableCollection<TaskItem>(_taskService.GetAll());
    }

    public ObservableCollection<TaskItem> Tasks { get; }

    public string? LastError
    {
        get => _lastError;
        private set
        {
            if (_lastError != value)
            {
                _lastError = value;
                OnPropertyChanged();
            }
        }
    }

    public bool TryAddTask(string? title)
    {
        try
        {
            var task = _taskService.AddTask(title ?? string.Empty);
            Tasks.Add(task);
            LastError = null;
            return true;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return false;
        }
    }

    public bool TryCompleteTask(Guid taskId)
    {
        try
        {
            var updated = _taskService.CompleteTask(taskId);
            var index = Tasks.IndexOf(Tasks.First(t => t.Id == taskId));
            Tasks[index] = updated;
            LastError = null;
            return true;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return false;
        }
    }

    public void Refresh()
    {
        Tasks.Clear();
        foreach (var task in _taskService.GetAll())
        {
            Tasks.Add(task);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
