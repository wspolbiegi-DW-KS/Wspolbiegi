namespace ReferenceApplicationArchitecture.DataTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.Data;
using ReferenceApplicationArchitecture.PresentationModel;

public class TaskRepositoryTests
{
    [Fact]
    public void Add_and_get_all_should_store_items_in_created_order()
    {
        var repo = new InMemoryTaskRepository();
        var t1 = new TaskItem(Guid.NewGuid(), "first", new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        var t2 = new TaskItem(Guid.NewGuid(), "second", new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc));

        repo.Add(t2);
        repo.Add(t1);

        repo.GetAll()
            .Select(t => t.Title)
            .Should().Equal("first", "second");
    }

    [Fact]
    public void Update_should_replace_existing_instance()
    {
        var repo = new InMemoryTaskRepository();
        var id = Guid.NewGuid();
        var original = new TaskItem(id, "title", DateTime.UtcNow);
        repo.Add(original);

        var updated = new TaskItem(id, "title", original.CreatedAt, isCompleted: true, completedAt: DateTime.UtcNow);
        repo.Update(updated);

        repo.GetById(id)!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void Remove_should_delete_when_present()
    {
        var repo = new InMemoryTaskRepository();
        var id = Guid.NewGuid();
        repo.Add(new TaskItem(id, "to-remove", DateTime.UtcNow));

        repo.Remove(id).Should().BeTrue();
        repo.GetById(id).Should().BeNull();
    }
}
