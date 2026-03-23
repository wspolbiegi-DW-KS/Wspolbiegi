namespace ReferenceApplicationArchitecture.BusinessLogicTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.BusinessLogic;
using ReferenceApplicationArchitecture.Data;
using ReferenceApplicationArchitecture.PresentationModel;

public class TaskServiceTests
{
    private readonly InMemoryTaskRepository _repository = new();

    [Fact]
    public void AddTask_should_normalize_and_store()
    {
        var service = CreateService();
        var task = service.AddTask("  hello  ");

        task.Title.Should().Be("hello");
        _repository.GetAll().Should().ContainSingle(t => t.Id == task.Id);
    }

    [Fact]
    public void AddTask_should_throw_on_empty_title()
    {
        var service = CreateService();
        var act = () => service.AddTask("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompleteTask_should_mark_done()
    {
        var clock = new Func<DateTime>(() => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        var service = CreateService(clock);
        var task = service.AddTask("test");

        var completed = service.CompleteTask(task.Id);

        completed.IsCompleted.Should().BeTrue();
        completed.CompletedAt.Should().Be(clock());
    }

    [Fact]
    public void Remove_should_delegate_to_repository()
    {
        var service = CreateService();
        var task = service.AddTask("to remove");

        service.Remove(task.Id).Should().BeTrue();
        _repository.GetById(task.Id).Should().BeNull();
    }

    private TaskService CreateService(Func<DateTime>? clock = null)
    {
        return new TaskService(_repository, clock);
    }
}
