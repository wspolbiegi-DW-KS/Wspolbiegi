namespace ReferenceApplicationArchitecture.PresentationModelTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.PresentationModel;

public class TaskItemTests
{
    [Fact]
    public void MarkCompleted_should_set_flags_and_timestamp()
    {
        var now = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var item = new TaskItem(Guid.NewGuid(), "demo", now);

        item.MarkCompleted(now);

        item.IsCompleted.Should().BeTrue();
        item.CompletedAt.Should().Be(now);
    }
}
