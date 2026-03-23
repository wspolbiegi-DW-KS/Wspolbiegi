namespace ReferenceApplicationArchitecture.PresentationViewModelTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.BusinessLogic;
using ReferenceApplicationArchitecture.PresentationModel;
using ReferenceApplicationArchitecture.PresentationViewModel;

public class TaskListViewModelTests
{
    [Fact]
    public void TryAddTask_should_append_when_service_succeeds()
    {
        var service = new FakeTaskService();
        var vm = new TaskListViewModel(service);

        vm.TryAddTask("demo").Should().BeTrue();
        vm.Tasks.Should().ContainSingle(t => t.Title == "demo");
        vm.LastError.Should().BeNull();
    }

    [Fact]
    public void TryAddTask_should_capture_error_when_service_throws()
    {
        var service = new FakeTaskService(shouldThrow: true);
        var vm = new TaskListViewModel(service);

        vm.TryAddTask("bad").Should().BeFalse();
        vm.LastError.Should().NotBeNull();
    }

    private sealed class FakeTaskService : ITaskService
    {
        private readonly bool _shouldThrow;
        private readonly List<TaskItem> _items = new();

        public FakeTaskService(bool shouldThrow = false)
        {
            _shouldThrow = shouldThrow;
        }

        public TaskItem AddTask(string title)
        {
            if (_shouldThrow)
            {
                throw new InvalidOperationException("boom");
            }

            var item = new TaskItem(Guid.NewGuid(), title, DateTime.UtcNow);
            _items.Add(item);
            return item;
        }

        public TaskItem CompleteTask(Guid id)
        {
            var item = _items.First(x => x.Id == id);
            item.MarkCompleted(DateTime.UtcNow);
            return item;
        }

        public IReadOnlyCollection<TaskItem> GetAll() => _items.ToList();

        public IReadOnlyCollection<TaskItem> GetOpen() => _items.Where(t => !t.IsCompleted).ToList();

        public bool Remove(Guid id) => _items.RemoveAll(t => t.Id == id) > 0;
    }
}
