namespace ReferenceApplicationArchitecture.DataTest;

using FluentAssertions;
using Xunit;
using ReferenceApplicationArchitecture.Data;

public class BallRepositoryTests
{
    [Fact]
    public void ReplaceAll_and_GetAll_should_store_given_balls()
    {
        var repo = new InMemoryBallRepository();
        var balls = new[]
        {
            new BallEntity(Guid.NewGuid(), 20, 20, 10, 0, 8),
            new BallEntity(Guid.NewGuid(), 50, 40, -10, 4, 8)
        };

        repo.ReplaceAll(balls);
        var loaded = repo.GetAll();

        loaded.Should().HaveCount(2);
        loaded.Select(b => b.Id).Should().Equal(balls[0].Id, balls[1].Id);
    }

    [Fact]
    public void GetAll_should_return_deep_copy()
    {
        var repo = new InMemoryBallRepository();
        var id = Guid.NewGuid();
        repo.ReplaceAll(new[] { new BallEntity(id, 30, 40, 1, 2, 8) });

        var snapshot1 = repo.GetAll();
        var snapshot2 = repo.GetAll();

        ReferenceEquals(snapshot1[0], snapshot2[0]).Should().BeFalse();
    }
}
